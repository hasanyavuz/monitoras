using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Monitoras.Entity {
    [Table ("MonitorStep")]
    public class MTDMonitorStep {
        [Key]
        public Guid MonitorStepId { get; set; }
        public Guid MonitorId { get; set; }
        public MTDMonitorStepTypes Type { get; set; }
        public string Settings { get; set; }

        public MTDSMonitorStepSettingsRequest SettingsAsRequest () {
            return JsonConvert.DeserializeObject<MTDSMonitorStepSettingsRequest> (Settings);
        }
    }

    public enum MTDMonitorStepTypes : short {
        Request = 1,
        StatusCode = 2,
        HeaderExists = 3,
        BodyContains = 4
    }

    public class MTDSMonitorStepSettingsRequest {
        public string Url { get; set; }
    }
}