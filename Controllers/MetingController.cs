using System;
using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using exampleWebAPI.Util;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MetingController : Controller
    {
        private readonly WeerstationContext _context;
        private readonly ResetToken _resetToken;
        private readonly HttpHeader _header;

        private User _user;

        public MetingController(WeerstationContext context)
        {
            _resetToken = new ResetToken();
            _context = context;
            CreateDb();
            _header = new HttpHeader();
        }

        public IActionResult Index()
        {
            return View();
        }





                [HttpGet("/all")]
                public IEnumerable<Meting> Get()
                {
                    return _context.Meting.ToList(); 
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
            var token = _resetToken.GetToken(_user).Result;
            if (token == null)
                if (_resetToken.ResetTokenNow(_user).Result)
                    token = _resetToken.GetToken(_user).Result;
            return token;
        }

        [HttpGet("/fakeData")]
        public string FakeData()
        {
            TokenValid();
            var ws = _context.Weerstation.Any(weerstation => weerstation.Id == 1)
                ? _context.Weerstation.First(ws1 => ws1.Id == 1)
                : new Weerstation("192.168.137.3", "arne1");
            _context.Weerstation.Update(ws);
            var m = new Meting
            {
                Illuminance = 1,
                Temperature = 1,
                Timestamp = DateTime.Now,
                Weatherstation = ws
            };
            _context.Meting.Add(m);

            return !_header.SendMeting(m, _user) ? "fout" : "ok";
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
            _user = _resetToken.Login(_user).Result;
            return _user.Cookies.ElementAt(1).Name;
        }

        [HttpGet("/GetVerToken")]
        public string GetVerToken()
        {
            _user = _resetToken.GetRequestVerificationToken(_user);
            return _user.Cookies.ElementAt(0).Name + " " + _user.Cookies.ElementAt(0).Value;
        }

        [HttpGet("/resetToken")]
        public string ResetToken()
        {
            return _resetToken.ResetTokenNow(_user).Result.ToString();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Meting value)
        {
            if (value == null)
            {
                return StatusCode(400, "fout ");
            }
            _context.Meting.Add(value);
            SendToJorg(value);
            return StatusCode(201, "Created");
        }

        private void SendToJorg(Meting value)
        {
            _header.SendMeting(value, _user);
        }
    }
}