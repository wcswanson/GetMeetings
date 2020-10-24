using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GetMeetings
    .Controllers
{
    public class HomeController : Controller
    {
        //// This block returns just the string without the view
        //public string Index()
        //{
        //    return "Hello Cruel World";
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}