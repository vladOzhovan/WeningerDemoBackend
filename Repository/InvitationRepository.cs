using WeningerDemoProject.Data;
using WeningerDemoProject.Models;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Dtos.Invitation;
using Microsoft.EntityFrameworkCore;

namespace WeningerDemoProject.Repository
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly AppDbContext _context;
        public InvitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invitation> CreateAsync(CreateInvitationDto dto)
        {
            var existing = await _context.Invitations.
                Where(i => i.Email == dto.Email && !i.IsUsed && !i.IsExpired).
                ToListAsync();

            foreach (var old in existing)
                old.UsedAt = DateTime.UtcNow;

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(dto.ValidTime)
            };

            await _context.Invitations.AddAsync(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }

        public async Task<Invitation?> GetByIdAsync(Guid id)
            => await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id);

        public async Task MarkUsedAsync(Invitation invitation)
        {
            invitation.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
