using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MetingController : Controller
    {
        private readonly WeerstationContext _context;
        private readonly HttpContext _httpContext;
        private readonly AuthenticationController _authenticationController;

        public MetingController()
        {
            _authenticationController = new AuthenticationController();
            _context = new WeerstationContext();
            _httpContext = new HttpContext();
        }

        [HttpGet("/graph")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public List<Meting> Get()
        {
            return _context.Meting.Include(m => m.Weatherstation).ToListAsync().Result;
        }


        [HttpGet("{id}")]
        public Meting Get(int id)
        {
            return _context.Meting.Include(m=> m.Weatherstation).FirstOrDefault(x => x.Id == id);
        }


        [HttpPost]
        public IActionResult Post([FromBody] Meting meting)
        {
            /* @todo
            aanpassen naar nette vorm zoals authcontrol signin()
            */
            if (meting == null)
                return StatusCode(417, "Meting is null");
            meting.Weatherstation =
                _context.Weerstation.FirstOrDefault(ws => meting.Weatherstation.Id == ws.Id);
            _context.Meting.Add(meting);
            _context.SaveChanges();
            return SendToJorg(meting) ? StatusCode(201, "Created") : StatusCode(400, "Bad Request");
        }

        private bool SendToJorg(Meting value)
        {
            return _httpContext.SendMeting(value, _authenticationController.GetUser());
        }
    }
}