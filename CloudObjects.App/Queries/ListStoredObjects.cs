using CloudObjects.Enums;
using CloudObjects.Interfaces;
using CloudObjects.Models;
using Dapper.QX.Abstract;
using Dapper.QX.Attributes;
using Dapper.QX.Interfaces;
using System.Collections.Generic;

namespace CloudObjects.App.Queries
{  
    public class ListStoredObjects : TestableQuery<StoredObject>, IListObjectsQuery
    {
        public ListStoredObjects() : base("SELECT * FROM [dbo].[StoredObject] WHERE [AccountId]=@accountId {andWhere} ORDER BY [Name] {offset}")
        {
        }

        public long AccountId { get; set; }

        [Where("[Name] LIKE CONCAT('%', @nameContains, '%')")]
        public string NameContains { get; set; }

        [Where("[Name] LIKE CONCAT(@nameStartsWith, '%')")]
        public string NameStartsWith { get; set; }

        [Offset(50)]
        public int? Page { get; set; }

        [OrderBy(ListStoredObjectsSortOptions.NameAscending, "[Name] ASC")]
        [OrderBy(ListStoredObjectsSortOptions.DateAscending, "COALESCE([DateModified], [DateCreated]) ASC")]
        [OrderBy(ListStoredObjectsSortOptions.DateDescending, "COALESCE([DateModified], [DateCreated]) DESC")]
        public ListStoredObjectsSortOptions Sort { get; set; }

        protected override IEnumerable<ITestableQuery> GetTestCasesInner()
        {
            yield return new ListStoredObjects() { AccountId = 5, NameContains = "whatever" };
            yield return new ListStoredObjects() { AccountId = 5, NameStartsWith = "whatever" };
            yield return new ListStoredObjects() { AccountId = 5, Sort = ListStoredObjectsSortOptions.DateDescending };
            yield return new ListStoredObjects() { AccountId = 5, Sort = ListStoredObjectsSortOptions.DateAscending };
            yield return new ListStoredObjects() { AccountId = 5, Sort = ListStoredObjectsSortOptions.NameAscending, Page = 2 };
        }
    }
}
