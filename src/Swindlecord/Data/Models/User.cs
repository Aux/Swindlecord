using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swindlecord
{
    [Table("users")]
    public class User
    {
        [Key]
        public ulong Id { get; set; }
        [Required]
        public string Username { get; set; }

        public User() { }
        public User(ulong userId, string username)
        {
            Id = userId;
            Username = username;
        }
    }
}
