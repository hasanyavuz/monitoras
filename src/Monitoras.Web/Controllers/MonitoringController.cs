using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitoras.Entity;
using Newtonsoft.Json;

namespace Monitoras.Web.Controllers {
    public class MonitoringController : ApiController {

        [NonAction]
        private async Task<object> GetMonitorClientModel (MTDMonitor monitor) {

            var url = String.Empty;

            var loadTime = 0.00;
            var loadTimes = new List<double> ();

            var totalMonitoredTime = 0;

            var upTime = 0.00;
            var upTimes = new List<double> ();

            var downTime = 0.00;
            var downTimePercent = 0.00;

            var stepStatus = MTDMonitorStepStatusTypes.Unknown;

            var monitorStepRequest = await Db.MonitorSteps.FirstOrDefaultAsync (m => m.MonitorId == monitor.MonitorId);
            if (monitorStepRequest != null) {
                var requestSettings = monitorStepRequest.SettingsAsRequest ();
                if (requestSettings != null) {
                    url = requestSettings.Url;
                }

                var week = DateTime.UtcNow.AddDays (-14);
                var logs = await Db.MonitorStepLogs
                    .Where (m => m.MonitorStepId == monitorStepRequest.MonitorStepId && m.StartDate >= week)
                    .OrderByDescending (m => m.StartDate)
                    .Take (50)
                    .ToListAsync ();

                logs = logs.OrderBy (x => x.StartDate).ToList ();

                if (logs.Any (m => m.Status == MTDMonitorStepStatusTypes.Success)) {
                    loadTime = logs.Where (m => m.Status == MTDMonitorStepStatusTypes.Success)
                        .Average (m => m.EndDate.Subtract (m.StartDate).TotalMilliseconds);
                }

                foreach (var log in logs) {
                    totalMonitoredTime += log.Interval;
                    if (log.Status == MTDMonitorStepStatusTypes.Success)
                        loadTimes.Add (log.EndDate.Subtract (log.StartDate).TotalMilliseconds);

                    if (log.Status == MTDMonitorStepStatusTypes.Fail)
                        downTime += log.Interval;

                    var currentDowntimePercent = (downTime / totalMonitoredTime) * 100;
                    var currentUptimePercent = 100 - currentDowntimePercent;

                    upTimes.Add (double.IsNaN (currentUptimePercent) ? 0 : currentUptimePercent);
                }

                var lastLog = logs.LastOrDefault ();
                if (lastLog != null) {
                    stepStatus = lastLog.Status;
                }

                downTimePercent = (downTime / totalMonitoredTime) * 100;
                upTime = 100 - downTimePercent;
            }

            if (double.IsNaN (upTime)) {
                upTime = 0;
            }

            return new {
                monitor.MonitorId,
                    monitor.CreatedDate,
                    monitor.LastCheckDate,
                    monitor.MonitorStatus,
                    monitor.Name,
                    monitor.TestStatus,
                    monitor.UpdatedDate,
                    url,
                    upTime,
                    upTimes,
                    downTime,
                    downTimePercent,
                    loadTime,
                    loadTimes,
                    totalMonitoredTime,
                    stepStatus,
                    stepStatustext = $"{stepStatus}"
            };
        }

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

                return Success (data: await GetMonitorClientModel (monitor));
            }

            var list = await Db.Monitors.Where (m => m.UserId == UserId).ToListAsync ();
            var clientList = new List<Object> ();

            foreach (var item in list) {
                clientList.Add (await GetMonitorClientModel (item));
            }

            return Success (null, clientList);
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
                    Settings = JsonConvert.SerializeObject (monitorStepData),
                    Interval = 10
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