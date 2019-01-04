namespace Recollectable.API.ViewModels
{
    public class AccountEmailViewModel
    {
        public string Url { get; set; }
        public string UserName { get; set; }

        public AccountEmailViewModel(string url, string userName)
        {
            Url = url;
            UserName = userName;
        }
    }
}