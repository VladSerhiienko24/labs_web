using BlackJack.Entities.Base;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackJack.Entities.Entities
{
    public class PlayerGame : BaseEntity
    {
        [Required]
        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        [Write(false)]
        [Computed]
        public Player Player { get; set; }

        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        [Write(false)]
        [Computed]
        public Game Game { get; set; }
    }
}