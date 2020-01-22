using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoras.Entity {
    [Table ("Monitor")]
    public class MTDMonitor {
        [Key]
        public Guid MonitorId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Name { get; set; }
        public MTDMonitorStatusTypes MonitorStatus { get; set; }
        public MTDTestStatusTypes TestStatus { get; set; }
        public DateTime LastCheckDate { get; set; }
        public decimal UpTime { get; set; }
        public int LoadTime { get; set; }
        public short MonitorTime { get; set; }
    }

    public enum MTDMonitorStatusTypes : short {
        Down = 0,
        Up = 1,
        Warning = 2
    }

    public enum MTDTestStatusTypes : short {
        Fail = 0,
        AllPassed = 1,
        Warning = 2
    }
}