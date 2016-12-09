using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class LogContext : DbContext
    {
        public DbSet<DiscordMessage> Messages { get; set; }


    }
}
