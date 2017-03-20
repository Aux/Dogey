using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogey.SQLite
{
    public class LiteDiscordMessage : LiteEntity<ulong>, IDiscordMessage<ulong>
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ulong? GuildId { get; set; }
        [Required]
        public ulong ChannelId { get; set; }
        [Required]
        public ulong MessageId { get; set; }
        [Required]
        public ulong AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Foreign Keys
        public LiteDiscordCommand Command { get; set; }
        public List<LiteDiscordReaction> Reactions { get; set; }

        public LiteDiscordMessage() { }
        public LiteDiscordMessage(SocketMessage message)
        {
            ChannelId = message.Channel.Id;
            AuthorId = message.Author.Id;
            MessageId = message.Id;
            Content = message.Content;
            Name = message.Author.Username;

            if (message.Channel is SocketTextChannel channel)
            {
                Name = (message.Author as SocketGuildUser)?.Nickname ?? message.Author.Username;
                GuildId = channel.Guild.Id;
            }
        }

        [NotMapped]
        ulong IDiscordMessage<ulong>.GuildId
            => throw new NotImplementedException();
    }
}
