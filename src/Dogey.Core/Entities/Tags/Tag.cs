using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dogey
{
    public class Tag : IEntity<ulong>, IOwnable<ulong>, ITimestamped
    {
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        public ulong Id { get; private set; }
        public ulong OwnerId { get; private set; }
        public ulong GuildId { get; private set; }
        public string Content { get; private set; }
        public string Names
        {
            get { return JsonConvert.SerializeObject(Aliases); }
            set { Aliases = JsonConvert.DeserializeObject<List<string>>(value); }
        }
        public List<string> Aliases { get; private set; }

        // Foreign Keys
        public List<TagLog> Logs { get; private set; }

        public Tag() { }
        public Tag(string name, string content, ICommandContext context)
        {
            Aliases = new List<string> { name };
            Content = content;
            GuildId = context.Guild.Id;
            OwnerId = context.User.Id;
        }

        internal void ResetUpdatedAt()
            => UpdatedAt = DateTime.UtcNow;

        internal void SetOwnerId(ulong ownerId)
        {
            OwnerId = ownerId;
            ResetUpdatedAt();
        }

        internal void SetContent(string content)
        {
            Content = content;
            ResetUpdatedAt();
        }

        internal void AddNames(string[] names)
        {
            Aliases.AddRange(names);
            ResetUpdatedAt();
        }

        internal void RemoveNames(string[] names)
        {
            foreach (var name in names)
                Aliases.Remove(name);
            ResetUpdatedAt();
        }
    }
}
