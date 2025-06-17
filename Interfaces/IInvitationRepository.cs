using WeningerDemoProject.Dtos.Invitation;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation> CreateAsync(CreateInvitationDto dto);
        Task<Invitation?> GetByIdAsync(Guid id);
        Task MarkUsedAsync(Invitation invitation);
    }
}
