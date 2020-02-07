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
        public int Interval { get; set; }
        public MTDMonitorStepStatusTypes Status { get; set; }

        public MTDSMonitorStepSettingsRequest SettingsAsRequest () {
            return JsonConvert.DeserializeObject<MTDSMonitorStepSettingsRequest> (Settings);
        }
    }

    public enum MTDMonitorStepStatusTypes : short {
        Unknown = 0,
        Pending = 1,
        Success = 2,
        Fail = 3,
        Warning = 4
    }

    public enum MTDMonitorStepTypes : short {
        Unknown = 0,
        Request = 1,
        StatusCode = 2,
        HeaderExists = 3,
        BodyContains = 4
    }

    public class MTDSMonitorStepSettingsRequest {
        public string Url { get; set; }
    }
}