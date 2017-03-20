using Discord.WebSocket;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogey.MySQL
{
    public class MyDiscordMessage : MyEntity<long>, IDiscordMessage<long>
    {
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long? GuildId { get; set; }
        [Required]
        public long ChannelId { get; set; }
        [Required]
        public long MessageId { get; set; }
        [Required]
        public long AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Content { get; set; }

        public MyDiscordMessage() { }
        public MyDiscordMessage(SocketMessage message)
        {
            ChannelId = (long)message.Channel.Id;
            AuthorId = (long)message.Author.Id;
            MessageId = (long)message.Id;
            Content = message.Content;
            Name = message.Author.Username;

            if (message.Channel is SocketTextChannel channel)
            {
                Name = (message.Author as SocketGuildUser)?.Nickname ?? message.Author.Username;
                GuildId = (long?)channel.Guild.Id;
            }
        }
        
        [NotMapped]
        long IDiscordMessage<long>.GuildId 
            => throw new NotImplementedException();
    }
}
