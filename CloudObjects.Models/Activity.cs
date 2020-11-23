using System;
using System.ComponentModel.DataAnnotations.Schema;
using CloudObjects.App.Models;

namespace CloudObjects.Models
{
    public enum Operation
    {
        Create,
        Update,
        Delete
    }

    [Table(nameof(Activity))]
    public class Activity : EntityBase<long>
    {
        public DateTime Timestamp { get; set; }

        public long AccountId { get; set; }
        public virtual Account Account { get; set; }
        
        public Operation Operation { get; set; }

        public long ObjectId { get; set; }
    }
}
