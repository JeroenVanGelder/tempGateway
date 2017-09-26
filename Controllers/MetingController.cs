using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using exampleWebAPI.Util;
using Microsoft.AspNetCore.Mvc;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MetingController : Controller
    {
        private readonly MetingContext _context;
        private HttpHeader _header = new HttpHeader();

        public MetingController(MetingContext context)
        {
            _context = context;
        }


        // GET
        [HttpGet]
        public IEnumerable<Meting> Get()
        {
            return _context.MetingItems.ToList();
        }

        // GET
        [HttpGet("{id}")]
        public Meting Get(int id)
        {
            return _context.MetingItems.FirstOrDefault(x => x.id == id);
        }

        //token
        [HttpGet("/token")]
        public string GetToken()
        {
            return HttpHeader.GetToken().Result.AccessToken;
        }


        //token
        [HttpGet("/fakeData")]
        public string fakeData()
        {
            Meting m = new Meting();
            m.Illuminance = 1;
            m.Temperature = 1;
            m.Timestamp = DateTime.Now;
            m.Weatherstation = "arne1";
            
            _header.CreateObject(m);
            
            return "";
        }


        [HttpPost]
        public IActionResult Post([FromBody] Meting value)
        {
            _header.CreateObject(value);

            _context.MetingItems.Add(value);
            _context.SaveChanges();
            return StatusCode(201, value);
        }
    }
}