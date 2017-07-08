using Discord.WebSocket;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swindlecord
{
    [Table("swindles")]
    public class Swindle
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public ulong UserId { get; set; }
        [Required]
        public ulong MessageId { get; set; }
        [Required]
        public string Content { get; set; }

        // Foreign Keys
        public List<SwindleStat> Stats { get; set; }

        public Swindle() { }
        public Swindle(long statusId, SocketUserMessage msg)
        {
            Id = statusId;
            UserId = msg.Author.Id;
            MessageId = msg.Id;
            Content = msg.Content;
        }
    }
}
