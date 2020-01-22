using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monitoras.Entity;

namespace Monitoras.Web.Controllers {
    public class MonitoringController : ApiController {
        [HttpGet]
        public IActionResult Get () {
            return Json (null);
        }

        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] MTDMonitor value) {
            if(string.IsNullOrEmpty(value.Name)){
                return Error("Name is required.");
            }

            var dataObject = new MTDMonitor {
                CreatedDate = DateTime.UtcNow,
                Name = value.Name
            };

            Db.Monitors.Add (dataObject);
            var result = await Db.SaveChangesAsync ();
            if (result > 0)
                return Success ("Monitoring saved successfully.", new {
                    Id = dataObject.MonitorId
                });
            else
                return Error ("Something is wrong with your model.");
        }
    }

    public class MonitoringModel {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}