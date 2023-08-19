using CloudBox.Api.Controllers.User;

namespace CloudBox.Api.Functions.User
{
    public interface IUserFunction
    {
        User? Authenticate(string loginId, string password);
        User GetUserById(int id);
        int Register(RegisterRequest request);
    }
}
