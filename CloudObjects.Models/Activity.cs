using AO.Models;
using CloudObjects.Models.Conventions;
using System;

namespace CloudObjects.Models
{
    public enum Operation
    {
        Create,
        Update,
        Delete
    }

    public class Activity : BaseTable
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [References(typeof(Account))]
        public long AccountId { get; set; }
        
        public Operation Operation { get; set; }

        public long ObjectId { get; set; }
    }
}
