using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Controllers
{
    public class ErrorMessageController : Controller
    {
        //For showing user friendly error messages upon faulty requests   
        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
        [Route("error/{code:int}")]
        public IActionResult Error(int code)
        {
            // handle different codes or just return the default error view
            return View();
        }

        [Route("account/accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}