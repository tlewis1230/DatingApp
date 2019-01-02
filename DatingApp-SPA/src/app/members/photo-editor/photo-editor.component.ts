import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from '../../_models/photo';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  // output Photo url (as string)
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  // only ONE drop zone
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService, private userService: UserService
    , private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      // ~10 MB
      maxFileSize: 10 * 1024 * 1024
    });

    // not uploading a file with credentials
    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };
    // building a photo object from the response from the server
     this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
        // if first photo uploaded
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          // this will override our user property inside our local storage with these new values
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      // find current main photo (use Array Filter method)
      // filter returns an array, AND we're always going to be returning 1 element in the array
      // so we need to refer to it AS an array element
      this.currentMain = this.photos.filter(p => p.isMain === true)[0];
      // & set to false
      this.currentMain.isMain = false;
      // send photo that's selected to be the main photo
      photo.isMain = true;
      this.authService.changeMemberPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      // this will override our user property inside our local storage with these new values
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      // this.getMemberPhotoChange.emit(photo.url);
    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto (id: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?',  () => {
       this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        // splice removes elements from an array
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertify.success('Photo has been deleted');
       }, error => {
        this.alertify.error('Failed to delete the photo');
       });
    });
  }
}
