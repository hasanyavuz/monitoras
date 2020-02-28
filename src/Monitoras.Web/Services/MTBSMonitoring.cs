using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitoras.Entity;

namespace Monitoras.Web {

    // Background Service
    public class MTBSMonitoring : IHostedService, IDisposable {

        public IServiceProvider Services { get; }
        private CancellationToken _token;
        public MTBSMonitoring (IServiceProvider services) {
            Services = services;
        }

        public Task StartAsync (CancellationToken cancellationToken) {
            _token = cancellationToken;
            DoWork ();
            return Task.CompletedTask;
        }

        private async void DoWork () {
            while (true) {
                using (var scope = Services.CreateScope ()) {

                    var db = scope.ServiceProvider.GetRequiredService<MTDContext> ();
                    var steps = await db.MonitorSteps
                        .Where (x => x.Type == MTDMonitorStepTypes.Request &&
                            x.Status != MTDMonitorStepStatusTypes.Processing &&
                            x.LastCheckDate.AddSeconds (x.Interval) > DateTime.UtcNow)
                        .OrderBy (x => x.LastCheckDate)
                        .Take (20)
                        .ToListAsync ();

                    foreach (var step in steps) {
                        var settings = step.SettingsAsRequest ();

                        var log = new MTDMonitorStepLog {
                            MonitorId = step.MonitorId,
                            MonitorStepId = step.MonitorStepId,
                            StartDate = DateTime.UtcNow,
                            Interval = step.Interval,
                            Status = MTDMonitorStepStatusTypes.Processing
                        };
                        db.Add (log);
                        await db.SaveChangesAsync (_token);

                        if (!string.IsNullOrEmpty (settings.Url)) {
                            try {
                                var client = new HttpClient ();
                                client.Timeout = TimeSpan.FromSeconds (15);
                                var result = await client.GetAsync (settings.Url, _token);

                                if (result.IsSuccessStatusCode) {
                                    log.Status = MTDMonitorStepStatusTypes.Success;
                                } else {
                                    log.Status = MTDMonitorStepStatusTypes.Fail;
                                }
                            } catch (HttpRequestException rex) {
                                log.Log = rex.Message;
                                log.Status = MTDMonitorStepStatusTypes.Fail;
                            } catch (Exception ex) {
                                log.Log = ex.Message;
                                log.Status = MTDMonitorStepStatusTypes.Error;
                            } finally {
                                log.EndDate = DateTime.UtcNow;
                            }

                            if (log.Status == MTDMonitorStepStatusTypes.Success) {
                                step.Status = MTDMonitorStepStatusTypes.Success;
                            } else if (log.Status == MTDMonitorStepStatusTypes.Error) {
                                step.Status = MTDMonitorStepStatusTypes.Error;
                            } else {
                                step.Status = MTDMonitorStepStatusTypes.Fail;
                            }
                        }
                        step.LastCheckDate = DateTime.UtcNow;
                        await db.SaveChangesAsync (_token);
                    }
                }
                await Task.Delay (500, _token);
            }
        }

        public Task StopAsync (CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public void Dispose () {

        }
    }
}