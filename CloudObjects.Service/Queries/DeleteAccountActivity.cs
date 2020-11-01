using Dapper.QX;

namespace CloudObjects.Service.Queries
{
    public class DeleteAccountActivity : Query<int>
    {
        public DeleteAccountActivity() : base("DELETE [dbo].[Activity] WHERE [AccountId]=@accountId")
        {
        }

        public long AccountId { get; set; }
    }
}
