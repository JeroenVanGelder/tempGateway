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
    public class JorgTokenController : Controller
    {
        private readonly MetingContext _context;

        public JorgTokenController(MetingContext context)
        {
            _context = context;
        }
        
        // GET
        [HttpGet]
        public string Get()
        {
            string returnValue = "";
            Console.WriteLine("Getting a new token");

            JorgServerConnecter jorgServerConnecter = new JorgServerConnecter();
            JorgToken newToken = jorgServerConnecter.getNewToken().Result;
            _context.JorgTokenItems.Add(newToken);
            _context.SaveChanges();
            
            return "OK";
        }
        
        //
        [HttpGet("{post}")]
        public string GetPost()
        {
            Meting meting = _context.MetingItems.Last();
            JorgToken lastToken = _context.JorgTokenItems.Last();
            JorgServerConnecter jorgServerConnecter = new JorgServerConnecter(lastToken);

            meting.Weatherstation = "J1";


                var httpPostResult = jorgServerConnecter.postMeting(meting).Result;

                return httpPostResult;
            }
        }
}