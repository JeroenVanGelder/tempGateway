using System;
using exampleWebAPI.Context;
using System.Collections.Generic;
using System.Linq;
using exampleWebAPI.Models;
using exampleWebAPI.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace exampleWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class JorgServerController : Controller
    {
        private readonly MetingContext _context;

        public JorgServerController(MetingContext context)
        {
            _context = context;
            
        }
        
        // GET
        [Obsolete("Breaking Call Structure",false)]
        [HttpGet]
        public string Get()
        {
            Console.WriteLine("Getting a new token");

            JorgServerConnecter jorgServerConnecter = new JorgServerConnecter();
            JorgToken newToken = jorgServerConnecter.UpdateToken().Result;

            if (newToken != null)
            {
                _context.JorgTokenItems.Add(newToken);
                _context.SaveChanges();
                return "OK";
            }

            return "Something went wrong";            
        }
        
        //
        [HttpGet("{post}")]
        public string PostLastMetingToJorgServer()
        {
            Meting meting = _context.MetingItems.Last();
            meting.Weatherstation = "J1";

            return PostMetingToJorgServer(meting);
        }

        public string PostMetingToJorgServer(Meting meting)
        {
            JorgServerConnecter jorgServerConnecter = new JorgServerConnecter(_context.GetLastJorgToken());
             
            var httpPostResult = jorgServerConnecter.postMeting(meting).Result;
            return httpPostResult;
        }
    }
}