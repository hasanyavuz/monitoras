using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoras.Entity {
    [Table ("MTDMonitorStepLog")]
    public class MTDMonitorStepLog {
        [Key]
        public Guid MonitorStepLogId { get; set; }
        public Guid MonitorId { get; set; }
        public Guid MonitorStepId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MTDMonitorStepStatusTypes Status { get; set; }
        public string Log { get; set; }
        public int Interval { get; set; }
    }
}