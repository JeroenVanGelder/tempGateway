using System;
using System.Linq;
using exampleWebAPI.Context;
using exampleWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace exampleWebAPI.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly WeerstationContext _context;
        private readonly TokenContext _tokenContext;

        private User _user;

        public AuthenticationController()
        {
            _tokenContext = new TokenContext();
            _context = new WeerstationContext();
            CreateUser();
        }

        public User GetUser()
        {
            return _user;
        }


        private void CreateUser()
        {
            if (_context.User.Count(u => u.Email.Equals("AR.Heil@student.han.nl")) > 0)
                _user = _context.User.Include(u => u.Token).First(user1 => user1.Email.Equals("AR.Heil@student.han.nl"));
            if (_user != null) return;
            _user = new User("AR.Heil@student.han.nl", "Pa@76iIL1b");
            _context.User.Add(_user);
            _context.SaveChanges();
        }

        [HttpPost]
        [Route("/signIn")]
        public ActionResult SignIn([FromBody] Weerstation weerstation)
        {
            if (weerstation == null || weerstation.Id == 0)
            {
                var ws0 = NewWeatherStation();
                var parseNewWeatherStation = ParseWeerstationToJson(ws0);

                Response.ContentLength = parseNewWeatherStation.Length;
                var url0 = "/api/Weatherstation/" + ws0.Id;

                return Created(url0, parseNewWeatherStation);
            }
            var ws = IsPresentInDb(weerstation);

            if (ws != null)
            {
                var parseWeatherStationExisting = ParseWeerstationToJson(ws);
                Response.ContentLength = parseWeatherStationExisting.Length;
                return Ok(parseWeatherStationExisting);
            }

            ws = NewWeatherStation();
            var parseWeatherStation = ParseWeerstationToJson(ws);
            Response.ContentLength = parseWeatherStation.Length;
            var url1 = "/api/Weatherstation/" + ws.Id;

            return Created(url1, parseWeatherStation);
        }
        [HttpOptions("/signIn")]
        public ActionResult GetOptionsSignin()
        {
            Response.Headers.Add("Allow", "OPTIONS, POST");

            return Ok();
        }

        private static string ParseWeerstationToJson(Weerstation ws)
        {
            return JsonConvert.SerializeObject(ws, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        private Weerstation NewWeatherStation()
        {
            var ws = new Weerstation { Name = RandomNameGenerator() };
            _context.Weerstation.Add(ws);
            _context.SaveChanges();
            ws.IpAddress = "192.168.137." + (ws.Id + 1);
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
            return _context.Weerstation.FirstOrDefault(weerstation1 => weerstation1.Id == weerstation.Id);
        }

        [HttpGet("/token")]
        public ActionResult GetToken()
        {
            var token = _tokenContext.GetToken(_user).Result;
            if (token != null)
            {
                var resp = ParseTokenToJson(token);
                Response.ContentLength = resp.Length;

                return Ok(resp);
            }
            if (_tokenContext.ResetTokenNow(_user).Result)
                token = _tokenContext.GetToken(_user).Result;


            var resp2 = ParseTokenToJson(token);
            Response.ContentLength = resp2.Length;
            return Ok(resp2);
        }
        [HttpOptions("/token")]
        public ActionResult GetOptionsToken()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET");

            return Ok();
        }

        [HttpGet("/isTokenValid")]
        public string TokenValid()
        {
            _user.Token = _tokenContext.GetToken(_user).Result;
            var resp = _user.Token.IsValid().ToString();
            Response.ContentLength = resp.Length;
            return resp;
        }
        [HttpOptions("/isTokenValid")]
        public ActionResult GetOptionsIsTokenValid()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET");

            return Ok();
        }

        [HttpGet("/resetToken")]
        public string ResetToken()
        {
            var t =  _tokenContext.ResetTokenNow(_user).Result.ToString();
            Response.ContentLength = t.Length;
            return t;
        }


        [HttpOptions("/resetToken")]
        public ActionResult GetOptionsResetToken()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET");

            return Ok();
        }

        private static string ParseTokenToJson(Token token)
        {
            return JsonConvert.SerializeObject(token, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}