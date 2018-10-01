using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    // http://localhost:5000/api/Values
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // inject DataContext in signatore
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;

        }
        // GET api/values
        [HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        public async Task<IActionResult> GetValues()
        {
            // values is our DBSet reference
            var values = await _context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
