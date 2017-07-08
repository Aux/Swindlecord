using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swindlecord
{
    [Table("statistics")]
    public class SwindleStat
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        [Required]
        public long SwindleId { get; set; }
        [Required]
        public ulong UserId { get; set; }
        [Required]
        public int Chances { get; set; }

        // Foreign Keys
        public Swindle Swindle { get; set; }

        public SwindleStat() { }
        public SwindleStat(long swindleId, ulong userId, int chances)
        {
            SwindleId = swindleId;
            UserId = userId;
            Chances = chances;
        }
    }
}
