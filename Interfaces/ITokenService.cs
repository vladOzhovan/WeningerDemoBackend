using WeningerDemoProject.Models;

namespace WeningerDemoProject.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
