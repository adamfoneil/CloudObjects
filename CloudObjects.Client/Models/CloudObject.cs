using CloudObjects.Models;
using System;

namespace CloudObjects.Client.Models
{
    public class CloudObject<T>
    {
        public CloudObject(T @object, StoredObject storedObject)
        {
            Object = @object;
            Name = storedObject.Name;
            Id = storedObject.Id;            
            Length = storedObject.Length;
            DateCreated = storedObject.DateCreated;
            DateModified = storedObject.DateModified;
        }

        public T Object { get; }
        public string Name { get; }
        public long Id { get; }
        public string Url { get; } // todo: make sure this gets set
        public long Length { get; }
        public DateTime DateCreated { get; }
        public DateTime? DateModified { get; }
    }
}
