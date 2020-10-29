namespace CloudObjects.App.ViewModels
{
    public class CreateLocalDbView
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string MessageClass => (Success) ? "is-success" : "is-danger";
        public string MessageTitle => (Success) ? "Success" : "Error";
    }
}
