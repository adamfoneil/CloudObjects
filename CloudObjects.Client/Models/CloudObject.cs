using System;

namespace CloudObjects.Client.Models
{
    public class CloudObject<T>
    {
        public T Object { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Url { get; set; }
        public long Length { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
