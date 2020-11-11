using Dapper.QX.Abstract;
using Dapper.QX.Interfaces;
using System.Collections.Generic;

namespace CloudObjects.App.Queries
{
    public class DeleteAllObjects : TestableQuery<int>
    {
        public DeleteAllObjects() : base("DELETE [dbo].[StoredObject] WHERE [AccountId]=@accountId")
        {
        }

        public long AccountId { get; set; }

        protected override IEnumerable<ITestableQuery> GetTestCasesInner()
        {
            yield return new DeleteAllObjects() { AccountId = -1 };
        }
    }
}
