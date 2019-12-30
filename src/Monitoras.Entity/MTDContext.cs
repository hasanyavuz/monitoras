using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Monitoras.Entity {
    public class MTDContext : IdentityDbContext<MTDUser>
    {
        public MTDContext(DbContextOptions options) : base(options)
        {
        }
    }
}