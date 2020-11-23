using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudObjects.App.Interfaces;

namespace CloudObjects.App.Models
{
    public abstract class EntityBase<TEntityKey> : IEntity<TEntityKey>
        where TEntityKey : struct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TEntityKey Id { get; set; }
    }
}
