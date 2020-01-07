using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Monitoras.Entity {
    public class MTDContext : IdentityDbContext<MTDUser, IdentityRole<Guid>, Guid>
    {
        public MTDContext(DbContextOptions options) : base(options)
        {
        }
    }
}