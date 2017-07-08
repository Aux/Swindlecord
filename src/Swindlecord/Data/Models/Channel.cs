using System.ComponentModel.DataAnnotations;

namespace Swindlecord
{
    public class Channel
    {
        [Key]
        public ulong Id { get; set; }
        [Required]
        public string Name { get; set; }

        public Channel() { }
        public Channel(ulong userId, string name)
        {
            Id = userId;
            Name = name;
        }
    }
}
