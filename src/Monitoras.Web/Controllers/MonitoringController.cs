using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitoras.Entity;
using Newtonsoft.Json;

namespace Monitoras.Web.Controllers {
    public class MonitoringController : ApiController {

        [HttpGet ("{id?}")]
        public async Task<IActionResult> Get ([FromRoute] Guid? id) {
            if (id.HasValue) {
                if (id.Value == Guid.Empty) {
                    return Error ("You must send monitor id to get.");
                }

                var monitor = await Db.Monitors.FirstOrDefaultAsync (m => m.MonitorId == id.Value && m.UserId == UserId);
                if (monitor == null) {
                    return Error ("Monitor not found.", code : 404);
                }

                var url = String.Empty;
                var monitorStepRequest = await Db.MonitorSteps.FirstOrDefaultAsync (m => m.MonitorId == monitor.MonitorId);
                if (monitorStepRequest != null) {
                    var requestSettings = monitorStepRequest.SettingsAsRequest ();
                    if (requestSettings != null) {
                        url = requestSettings.Url;
                    }
                }

                return Success (data: new {
                    monitor.MonitorId,
                        monitor.CreatedDate,
                        monitor.LastCheckDate,
                        monitor.MonitorStatus,
                        monitor.Name,
                        monitor.TestStatus,
                        monitor.UpTime,
                        monitor.UpdatedDate,
                        Url = url
                });
            }

            var list = await Db.Monitors.ToListAsync ();
            return Success (null, list);
        }

        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] MTMMonitorSave value) {
            if (string.IsNullOrEmpty (value.Name)) {
                return Error ("Name is required.");
            }

            var monitorCheck = await Db.Monitors.AnyAsync (m => m.MonitorId != value.Id && m.Name.Equals (value.Name) && m.UserId == UserId);
            if (monitorCheck) {
                return Error ("This project name is already in use. Plase choose a different name.");
            }

            MTDMonitor data = null;
            if (value.Id != Guid.Empty) {
                data = await Db.Monitors.FirstOrDefaultAsync (m => m.MonitorId == value.Id && m.UserId == UserId);
                if (data == null) {
                    return Error ("Monitor not found.");
                }

                data.UpdatedDate = DateTime.UtcNow;
                data.Name = value.Name;
            } else {
                data = new MTDMonitor {
                    MonitorId = Guid.NewGuid (),
                    CreatedDate = DateTime.UtcNow,
                    Name = value.Name,
                    UserId = UserId
                };
                Db.Monitors.Add (data);
            }

            var monitorStepData = new MTDSMonitorStepSettingsRequest {
                Url = value.Url
            };

            var step = await Db.MonitorSteps.FirstOrDefaultAsync (m => m.MonitorId == data.MonitorId);
            if (step != null) {
                var requestSettings = step.SettingsAsRequest () ?? new MTDSMonitorStepSettingsRequest ();
                requestSettings.Url = value.Url;
                step.Settings = JsonConvert.SerializeObject (requestSettings);
            } else {
                step = new MTDMonitorStep {
                    MonitorStepId = Guid.NewGuid (),
                    Type = MTDMonitorStepTypes.Request,
                    MonitorId = data.MonitorId,
                    Settings = JsonConvert.SerializeObject (monitorStepData)
                };
                Db.MonitorSteps.Add (step);
            }

            var result = await Db.SaveChangesAsync ();

            if (result > 0)
                return Success ("Monitoring saved successfully.", new {
                    Id = data.MonitorId
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