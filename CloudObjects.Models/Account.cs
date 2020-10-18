using AO.Models;
using AO.Models.Enums;
using CloudObjects.Models.Conventions;
using System;
using System.ComponentModel.DataAnnotations;

namespace CloudObjects.Models
{
    public partial class Account : AuditedTable
    {
        [Key]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        [SaveAction(SaveAction.Insert)]
        public string Key { get; set; }

        [SaveAction(SaveAction.Insert)]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow.AddDays(30);

        public override long AuditAccountId => Id;

        public override long AuditObjectId => 0;
    }
}
