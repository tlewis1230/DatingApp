using AutoMapper;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
   //[Authorize]
    // no methods in the class will be available to anonymous users
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;            
            _repo = repo;

            // set up Cloudinary Account
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );


            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        // userid comes from route params
        // [FromForm] - using a form to send our photo
        public async Task<IActionResult> AddPhotoForUser(int userId,
            [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            // need to compare the userid  in the token against the userid in the route 
            // since this is an authorized call
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId,true);

            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())    
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream), 
                        //crop the photo with this call:
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }

            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;
            // map PhotoForCreationDto into Photo
            var photo = _mapper.Map<Photo>(photoForCreationDto);
            // is this the first photo?
            if(!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            
            userFromRepo.Photos.Add(photo);
            
            if(await _repo.SaveAll())
            {
                // return Ok();
                // returns an HttpStatus Code of 201
                // guarantee if save is successfull our photo will have an id of what
                // sqlite generates and that will be stored in our PhotoToReturn
                var photoToReturn =  _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
            }
            // if we have an error on save
            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            // need to compare the userid  in the token against the userid in the route 
            // since this is an authorized call
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // check that the id of the photo belongs to the user
            var user = await _repo.GetUser(userId,true);
            // check that the photo exists in the user's photo collection
            if (!user.Photos.Any(p => p.Id == id))            
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);
            // is this photo already the main photo?
            if (photoFromRepo.IsMain)
                return BadRequest("This Photo Is Already the Main Photo.");
            
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            
            // set current main photo to false
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could Not Set Photo To Main");
        }

        [HttpDelete("{id}")]
        // Can't delete the main photo
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // need to compare the userid  in the token against the userid in the route 
            // since this is an authorized call
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // check that the id of the photo belongs to the user
            var user = await _repo.GetUser(userId, true);
            // check that the photo exists in the user's photo collection
            if (!user.Photos.Any(p => p.Id == id))            
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            // is this photo already the main photo?
            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo.");           
            
            // if photo is from cloudinary
            if (photoFromRepo.PublicId != null)
            {
                // create deletion params to pass into Cloudinary 
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);            
            
                // need to delete photo from cloudinary 
                var result = _cloudinary.Destroy(deleteParams);
                
                if (result.Result == "ok") {
                    // and from Sqlite 
                    _repo.Delete(photoFromRepo);
                }   
            }
            
            // if photo is NOT from cloudinary
            if (photoFromRepo.PublicId == null)
            {
                  _repo.Delete(photoFromRepo);
            }
                       
            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}