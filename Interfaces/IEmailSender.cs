namespace WeningerDemoProject.Interfaces
{
    public interface IEmailSender
    {
        Task SendInvitationAsync(string toEmail, string regLink);
    }
}
