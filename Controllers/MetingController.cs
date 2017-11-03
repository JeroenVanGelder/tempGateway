using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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


        [HttpGet]
        public IActionResult Get()
        {
            var ws0 = _context.Meting.Include(m => m.Weatherstation).ToListAsync().Result;
            if (ws0 != null)
            {
                var rep = ParseMetingenToJson(ws0);
                Response.ContentLength = rep.Length;
                return Ok(rep);
            }
            return NoContent();



        }



        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var m0 =  _context.Meting.Include(m => m.Weatherstation).FirstOrDefault(x => x.Id == id);
            if (m0 != null)
            {
                var rep = ParseMetingToJson(m0);
                Response.ContentLength = rep.Length;
                return Ok(rep);
            }
            return NoContent();
        }


        [HttpPost]
        public IActionResult Post([FromBody] Meting meting)
        {
            if (meting == null)
            {
                var msg = "meting is null";
                Response.ContentLength = msg.Length;
                return BadRequest(msg);
            }

            meting.Weatherstation =
                _context.Weerstation.FirstOrDefault(ws => meting.Weatherstation.Id == ws.Id);
            _context.Meting.Add(meting);
            _context.SaveChanges();
            if (SendToJorg(meting))
            {
                var metingJson = ParseMetingToJson(meting);
                Response.ContentLength = metingJson.Length;
                var newMetingUri = "/api/meting/" + meting.Id;
                return Created(newMetingUri, metingJson);
            }
            else
            {
                var msg = "versturen mislukt";
                Response.ContentLength = msg.Length;
                return BadRequest(msg);
            }
        }

        private bool SendToJorg(Meting value)
        {
            return _httpContext.SendMeting(value, _authenticationController.GetUser());
        }

        private static string ParseMetingToJson(Meting meting)
        {
            return JsonConvert.SerializeObject(meting, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
        [HttpOptions]
        public ActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST");

            return Ok();
        }




        [HttpGet("/api/meting/graph")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpOptions("/api/meting/graph")]
        public ActionResult GetOptionsGraph()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET");

            return Ok();
        }

        private static string ParseMetingenToJson(List<Meting> m)
        {
            return JsonConvert.SerializeObject(m, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

    }
}