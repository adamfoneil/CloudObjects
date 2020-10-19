using Dapper.QX;

namespace CloudObjects.App.Queries
{
    public class DeleteAccountActivity : Query<int>
    {
        public DeleteAccountActivity() : base("DELETE [dbo].[Activity] WHERE [AccountId]=@accountId")
        {
        }

        public long AccountId { get; set; }
    }
}
