using System.ComponentModel.DataAnnotations;

namespace Swindlecord
{
    public class Guild
    {
        [Key]
        public ulong Id { get; set; }
        [Required]
        public string Name { get; set; }

        public Guild() { }
        public Guild(ulong userId, string name)
        {
            Id = userId;
            Name = name;
        }
    }
}
