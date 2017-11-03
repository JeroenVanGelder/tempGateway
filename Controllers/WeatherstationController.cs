using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public ActionResult Get()
        {
            var ws0 = _context.Weerstation.ToList();
            if (ws0 != null)
            {
                var rep = ParseWeerstationsToJson(ws0);
                Response.ContentLength = rep.Length;
                return Ok(rep);
            }
            return NoContent();
        }


        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {

            var ws0 = _context.Weerstation.FirstOrDefault(x => x.Id == id);
            if (ws0 != null)
            {
                var rep = ParseWeerstationToJson(ws0);
                Response.ContentLength = rep.Length;
                return Ok(rep);
            }
            return NoContent();
        }

        [HttpOptions]
        public ActionResult GetOptions()
        {
            Response.Headers.Add("Allow","OPTIONS, GET");

            return Ok();
        }
        private static string ParseWeerstationToJson(Weerstation ws)
        {
            return JsonConvert.SerializeObject(ws, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        private static string ParseWeerstationsToJson(List<Weerstation> ws)
        {
            return JsonConvert.SerializeObject(ws, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}