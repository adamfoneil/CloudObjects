using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudObjects.App.Models;

namespace CloudObjects.Models
{
    [Table(nameof(StoredObject))]
    public class StoredObject : AuditedEntity<long>
    {
        public long AccountId { get; set; }
        public virtual Account Account { get; set; }

        [MaxLength(512)]
        public string Name { get; set; }

        [Required]
        public string Json { get; set; }

        public long Length { get; set; }
        
        public override long AuditAccountId => AccountId;

        public override long AuditObjectId => Id;
    }
}
