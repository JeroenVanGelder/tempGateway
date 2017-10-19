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
        public Token GetToken()
        {
            var token = _tokenContext.GetToken(_user).Result;
            if (token != null) return token;
            if (_tokenContext.ResetTokenNow(_user).Result)
                token = _tokenContext.GetToken(_user).Result;
            return token;
        }

        [HttpGet("/isTokenValid")]
        public string TokenValid()
        {
            _user.Token = GetToken();
            return _user.Token.IsValid().ToString();
        }

        [HttpGet("/resetToken")]
        public string ResetToken()
        {
            return _tokenContext.ResetTokenNow(_user).Result.ToString();
        }
    }
}