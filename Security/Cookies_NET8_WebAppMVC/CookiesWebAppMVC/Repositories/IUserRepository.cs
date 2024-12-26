using CookiesWebAppMVC.Models;

namespace CookiesWebAppMVC.Repositories;

public interface IUserRepository
{
    UserModel? GetByUsernameAndPassword(string username, string password);
    UserModel? GetByGoogleId(string googleId);
    List<UserModel> GetAllUsers();
}