namespace CloudObjects.App.Exception
{
    public class EntityNotFoundException : System.Exception
    {
        public EntityNotFoundException(string message = null) : base(message)
        {
        }
    }
}
