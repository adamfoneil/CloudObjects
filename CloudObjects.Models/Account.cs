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

        /// <summary>
        /// if we were going to make a monetized service, then I figured we would want 
        /// to track some kind of renewal date, but this is not enforced at all currently
        /// </summary>
        [SaveAction(SaveAction.Insert)]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow.AddDays(30);

        public override long AuditAccountId => Id;

        public override long AuditObjectId => 0;
    }
}
