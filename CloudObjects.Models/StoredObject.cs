using AO.Models;
using AO.Models.Enums;
using CloudObjects.Models.Conventions;
using System.ComponentModel.DataAnnotations;

namespace CloudObjects.Models
{
    public class StoredObject : AuditedTable
    {
        [References(typeof(Account))]
        [Key]
        [SaveAction(SaveAction.Insert)]
        public long AccountId { get; set; }

        [Key]
        [MaxLength(512)]
        public string Name { get; set; }

        [Required]
        public string Json { get; set; }

        public long Length { get; set; }

        public override long AuditAccountId => AccountId;

        public override long AuditObjectId => Id;
    }
}
