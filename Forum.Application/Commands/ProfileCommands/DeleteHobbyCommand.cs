using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Commands.ProfileCommands
{
    public class DeleteHobbyCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public Guid HobbyId { get; set; }
    }
}