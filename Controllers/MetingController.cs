using System;
using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MetingController : Controller
    {
        private readonly WeerstationContext _context;
        private readonly TokenContext _tokenContext;
        private readonly HttpContext _httpContext;


        private User _user;

        public MetingController()
        {
            _tokenContext = new TokenContext();
            _context = new WeerstationContext();
            CreateDb();
            _httpContext = new HttpContext();
        }


        [HttpGet("/graph")]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IEnumerable<Meting> Get()
        {
            return _context.Meting.ToList();
        }

        [HttpGet("/getAllWs")]
        public IEnumerable<Weerstation> GetWs()
        {
            return _context.Weerstation.ToList();
        }


        [HttpGet("{id}")]
        public Meting Get(int id)
        {
            return _context.Meting.FirstOrDefault(x => x.Id == id);
        }


        [HttpGet("/CreateDB")]
        public string CreateDb()
        {
            if (_context.User.Any(user1 => user1.Email == "AR.Heil@student.han.nl"))
            {
                _user = _context.User.First(user1 => user1.Email == "AR.Heil@student.han.nl");
                return "oke";
            }
            _user = new User("AR.Heil@student.han.nl", "Pa@76iIL1b");
            _context.User.Update(_user);
            _context.SaveChanges();
            return "oke na aanmaken";
        }


        [HttpGet("/token")]
        public Token GetToken()
        {
            var token = _tokenContext.GetToken(_user).Result;
            if (token != null) return token;
            if (_tokenContext.ResetTokenNow(_user).Result)
                token = _tokenContext.GetToken(_user).Result;
            return token;
        }

        [HttpGet("/fakeData")]
        public string FakeData()
        {
            //   TokenValid();
            var ws = _context.Weerstation.Any(weerstation => weerstation.Id == 1)
                ? _context.Weerstation.First(ws1 => ws1.Id == 1)
                : new Weerstation("192.168.137.3", "arne1");
            //_context.Weerstation.Add(ws);
            var m = new Meting
            {
                Illuminance = 1,
                Temperature = 1,
                Timestamp = DateTime.Now,
                Weatherstation = ws
            };
            _context.Meting.Add(m);
            _context.SaveChanges();
            return "oke";
            //            return !_header.SendMeting(m, _user) ? "fout" : "ok";
        }

        [HttpGet("/isTokenValid")]
        public string TokenValid()
        {
            _user.Token = GetToken();
            return _user.Token.IsValid().ToString();
        }

        [HttpGet("/mockLogin")]
        public string MockLogin()
        {
            _user = _tokenContext.Login(_user).Result;
            return _user.Cookies.ElementAt(1).Name;
        }

        [HttpGet("/GetVerToken")]
        public string GetVerToken()
        {
            _user = _tokenContext.GetRequestVerificationToken(_user);
            return _user.Cookies.ElementAt(0).Name + " " + _user.Cookies.ElementAt(0).Value;
        }

        [HttpGet("/resetToken")]
        public string ResetToken()
        {
            return _tokenContext.ResetTokenNow(_user).Result.ToString();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Meting value)
        {
            if (value == null)
                return StatusCode(417, "Meting is null");
            _context.Meting.Add(value);
            SendToJorg(value);
            return StatusCode(201, "Created");
        }

        private void SendToJorg(Meting value)
        {
            _httpContext.SendMeting(value, _user);
        }

        [HttpPost]
        [Route("/signIn")]
        public ActionResult SignIn([FromBody] Weerstation weerstation)
        {
            if (weerstation.Id == 0)
                return Created("uri", ParseWeerstationToJson(NewWeatherStation()));


            var ws = IsPresentInDb(weerstation);

            if (ws != null)
            {
                var parseWeatherStationExisting = ParseWeerstationToJson(ws);
                Response.ContentLength = parseWeatherStationExisting.Length;
                return Ok(parseWeatherStationExisting);
            }

            var parseWeatherStation = ParseWeerstationToJson(NewWeatherStation());
            Response.ContentLength = parseWeatherStation.Length;
            return Created("URI", parseWeatherStation);
        }

        private static string ParseWeerstationToJson(Weerstation ws)
        {
            return JsonConvert.SerializeObject(ws, Formatting.None,
                new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        }

        private Weerstation NewWeatherStation()
        {
            var ws = new Weerstation {Name = RandomNameGenerator()};
            _context.Weerstation.Add(ws);
            _context.SaveChanges();
            ws.IpAddress = "10.42.0." + (ws.Id + 1);
            _context.Weerstation.Update(ws);
            _context.SaveChanges();
            return ws;
        }

        private static string RandomNameGenerator()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];

            return new string(stringChars);
        }

        private Weerstation IsPresentInDb(Weerstation weerstation)
        {
            var ws = _context.Weerstation.FirstOrDefault(weerstation1 => weerstation1.Id == weerstation.Id);
            if (ws == null) return null;
            return ws;
        }
    }
}