﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    // Note: ControllerBase does NOT Have View support
    // (the Controller class has this)
    public class ValuesController : ControllerBase
    {
        // inject DataContext in signature
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;

        }
        // GET api/values
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            // values is our DBSet reference
            var values = await _context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [Authorize(Roles = "Member")]
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
