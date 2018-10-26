using BlackJack.Entities.Base;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackJack.Entities.Entities
{
    public class HandCard : BaseEntity
    {
        [Required]
        public int HandId { get; set; }
        [ForeignKey("HandId")]
        [Write(false)]
        [Computed]
        public Hand Hand { get; set; }

        [Required]
        public int CardId { get; set; }
        [ForeignKey("CardId")]
        [Write(false)]
        [Computed]
        public Card Card { get; set; }
    }
}