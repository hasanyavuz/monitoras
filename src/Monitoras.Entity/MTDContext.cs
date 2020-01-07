using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Monitoras.Entity {
    public class MTDContext : IdentityDbContext<MTDUser, IdentityRole<Guid>, Guid> {
        public MTDContext (DbContextOptions options) : base (options) { }

        public DbSet<MTDMonitor> Monitors { get; set; }
        public DbSet<MTDMonitorStep> MonitorSteps { get; set; }
        public DbSet<MTDMonitorStepLog> MonitorStepLogs { get; set; }
    }
}