using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Targets;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    [Authorize("ElevatedRights")]
    public class NLogController : Controller
    {

        private readonly IHostingEnvironment env;

        public NLogController(IHostingEnvironment env)
        {
            this.env = env;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult GetLogByDate(DateTime shortDate)
        {

            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("ownFile-web");
            var logEventInfo = new LogEventInfo { TimeStamp = shortDate };
            string fileName = fileTarget.FileName.Render(logEventInfo);

            if (!System.IO.File.Exists(fileName))
                return NotFound("File does not exist");
            else
                return PartialView("_DisplayLog", FormatColorVM(System.IO.File.ReadAllLines(fileName)));

        }

        private NlogIndexVM[] FormatColorVM(string[] logs)
        {

            /* 
             * Available Log Commands
             * 
             * Critical
             * Debug
             * Error
             * LogInformation
             * Trace
             * Warning
             * 
             */

            var model = new List<NlogIndexVM>();

            foreach (var log in logs)
            {

                var logType = log.Split('|')[1];
                var newLog = new NlogIndexVM();
                newLog.Log = log;

                switch (logType)
                {
                    case "FATAL":
                        newLog.HexCode = "#b73946";
                        break;
                    case "ERROR":
                        newLog.HexCode = "#c98661";
                        break;
                    case "INFO":
                        newLog.HexCode = "#d3ffce";
                        break;
                    case "WARN":
                        newLog.HexCode = "#feee99";
                        break;
                    default:
                        newLog.HexCode = "#fff";
                        break;
                }

                model.Add(newLog);

            }

            return model.ToArray();
        } 

        [HttpDelete]
        public IActionResult DeleteLogByDate(DateTime shortDate)
        {

            string pathToLog = String.Empty;

            if (env.IsDevelopment())
            {
                pathToLog = Path.Combine(env.ContentRootPath, $"log-{shortDate}.log");
            }
            else if (env.IsProduction())
            {
                pathToLog = Path.Combine(env.WebRootPath, $"log-{shortDate}.log");
            }

            if (System.IO.File.Exists(pathToLog))
            {

                try
                {
                    System.IO.File.Delete(pathToLog);
                    return Ok("File successfully deleted");
                }
                catch (Exception e)
                {
                    return BadRequest("Internal server error");
                }

            }
            else
                return NotFound("File does not exist");

        }

    }
}
