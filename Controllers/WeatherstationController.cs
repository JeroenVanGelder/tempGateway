using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class WeatherstationController : Controller
    {
        private readonly WeerstationContext _context;

        public WeatherstationController()
        {
            _context = new WeerstationContext();
        }

        [HttpGet]
        public IEnumerable<Weerstation> Get()
        {
            return _context.Weerstation.ToList();
        }


        [HttpGet("{id}")]
        public Weerstation Get(int id)
        {
            return _context.Weerstation.FirstOrDefault(x => x.Id == id);
        }
    }
}