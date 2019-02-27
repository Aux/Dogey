using System;

namespace Dogey.Models
{
    /// <summary> Specifies a service's interest in an event </summary>
    public enum ReactionInterest
    {
        None,
        ReactionRole,
        Starboard,
        Voting
    }

    /// <summary>  </summary>
    public class ReactionCondition
    {
        public ulong Id { get; set; }

        public ReactionInterest Interest { get; set; }
        public string Name { get; set; }
        public ulong? GuildId { get; set; }
        public ulong? ChannelId { get; set; }
        public ulong? MessageId { get; set; }
        public ulong? UserId { get; set; }
    }
}
