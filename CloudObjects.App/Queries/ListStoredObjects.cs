using CloudObjects.Enums;

namespace CloudObjects.App.Queries
{  
    public class ListStoredObjects
    {
        public string NameContains { get; set; }
        public string NameStartsWith { get; set; }
        public int PageSize => 50;
        public int? Page { get; set; }
        public ListStoredObjectsSortOptions Sort { get; set; }

    }
}
