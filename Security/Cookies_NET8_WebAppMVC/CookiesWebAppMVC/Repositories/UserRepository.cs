using CookiesWebAppMVC.Extensions;
using CookiesWebAppMVC.Models;

namespace CookiesWebAppMVC.Repositories;

public class UserRepository : IUserRepository
{
    private List<UserModel> users = new()
    {
        new UserModel
        {
            Id = 3522, Name = "max", Password = "7NcYcNGWMxapfjrDQIyYNa2M8PPBvHA1J8MCZVNPda4=",
            FavoriteColor = "blue", Role = "Admin", GoogleId = "101517359495305583936"
        },new UserModel
        {
            Id = 3523, Name = "maude", Password = "7NcYcNGWMxapfjrDQIyYNa2M8PPBvHA1J8MCZVNPda4=",
            FavoriteColor = "purple", Role = "Chief", GoogleId = "101517359495305583937"
        },
    };

    public UserModel? GetByUsernameAndPassword(string username, string password)
    {
        var encryptedPwd = password.Sha256();
        var user = users.SingleOrDefault(u => u.Name == username && u.Password == encryptedPwd);
        return user;
    }

    public UserModel? GetByGoogleId(string googleId)
    {
        var user = users.SingleOrDefault(u => u.GoogleId == googleId);
        return user;
    }

    public List<UserModel> GetAllUsers()
    {
        return users;
    }
}