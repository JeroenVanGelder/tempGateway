using System;

using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MetingController : Controller
    {
        private readonly MetingContext _context;
        private JorgServerController _jorgServerController;
        
        public MetingController(MetingContext context)
        {
            _context = context;
            _jorgServerController = new JorgServerController(_context);
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

        [HttpPost]
        public String Post([FromBody]Meting value)
        {
            [FromBody]
            value.Timestamp=DateTime.Now;
            _context.MetingItems.Add(value);
            _context.SaveChanges();
            
            
            
            var statusCode = _jorgServerController.PostMetingToJorgServer(value);
            return statusCode;
        }
    }
}