using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Models
{
    public class UserBans
    {
        public ulong ID { get; set; }
        public Dictionary<ulong, int> Bans { get; set; }
        public List<ulong> Servers
        {
            get { return this.Servers; }
            set
            {
                for (int i = 0; i < Servers.Count(); i++)
                {
                    if (!this.Bans.Keys.Contains(Servers[i]))
                    {
                        Bans.Add(Servers[i], 1);
                    }
                }
            }
        }
        public int TotalBans { get { return Bans.Sum(x => x.Value); } }

        public UserBans()
        {
            Servers = new List<ulong>();
            Bans = new Dictionary<ulong, int>();
        }
    }
}
