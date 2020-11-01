using CloudObjects.Models;
using System;
using System.Text.Json;

namespace CloudObjects.Service.Models
{
    public class CloudObject<T>
    {
        public static CloudObject<T> FromStoredObject(StoredObject storedObject)
        {
            var obj = JsonSerializer.Deserialize<T>(storedObject.Json);
            return new CloudObject<T>(obj, storedObject);
        }

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
