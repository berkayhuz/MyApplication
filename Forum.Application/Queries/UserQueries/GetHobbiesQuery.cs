using Forum.Application.Common.DTOs;
using MediatR;

namespace Forum.Application.Queries.UserQueries
{
    public class GetHobbiesQuery : IRequest<List<UserHobbyDto>>
    {
        public Guid UserId { get; set; }
    }
}
