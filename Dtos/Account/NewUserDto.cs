namespace WeningerDemoProject.Dtos.Account
{
    public class NewUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public IList<string> Roles { get; set; } = null!;
    }
}
