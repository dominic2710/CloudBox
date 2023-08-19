namespace CloudBox.Api.Controllers.User
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
