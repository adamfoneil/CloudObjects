using CloudObjects.Enums;
using CloudObjects.Interfaces;

namespace CloudObjects.Client.Models
{
    public class ListObjectsQuery : IListObjectsQuery
    {
        public string NameContains { get; set; }
        public string NameStartsWith { get; set; }
        public ListStoredObjectsSortOptions Sort { get; set; }
        public int? Page { get; set; }
    }
}
