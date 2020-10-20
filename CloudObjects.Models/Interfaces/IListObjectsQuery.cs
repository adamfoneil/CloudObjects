using CloudObjects.Enums;

namespace CloudObjects.Interfaces
{
    public interface IListObjectsQuery
    {
        string NameContains { get; set; }
        string NameStartsWith { get; set; }
        int? Page { get; set; }
        ListStoredObjectsSortOptions Sort { get; set; }
    }
}
