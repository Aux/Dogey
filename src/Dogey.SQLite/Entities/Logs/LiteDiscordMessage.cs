using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class LiteDiscordMessage : LiteEntity<ulong>, IDiscordMessage
    {
        [Required]
        public DateTime CreatedAt { get; set; }
        public ulong? GuildId { get; set; }
        [Required]
        public ulong ChannelId { get; set; }
        [Required]
        public ulong MessageId { get; set; }
        [Required]
        public ulong AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

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
            CreatedAt = message.Timestamp.UtcDateTime;

            if (message.Channel is SocketTextChannel channel)
            {
                Name = (message.Author as SocketGuildUser)?.Nickname ?? message.Author.Username;
                GuildId = channel.Guild.Id;
            }
        }
    }
}
