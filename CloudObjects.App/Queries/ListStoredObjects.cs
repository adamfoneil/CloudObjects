using CloudObjects.Models;
using Dapper.QX.Abstract;
using Dapper.QX.Attributes;
using Dapper.QX.Interfaces;
using System.Collections.Generic;

namespace CloudObjects.App.Queries
{
    public class ListStoredObjects : TestableQuery<StoredObject>
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

        protected override IEnumerable<ITestableQuery> GetTestCasesInner()
        {
            yield return new ListStoredObjects() { AccountId = 5, NameContains = "whatever" };
            yield return new ListStoredObjects() { AccountId = 5, NameStartsWith = "whatever" };
        }
    }
}
