using Forum.Application.Common.DTOs;
using MediatR;

namespace Forum.Application.Queries.UserQueries
{
    public class GetSocialMediaLinksQuery : IRequest<List<UserSocialMediaDto>>
    {
        public Guid UserId { get; set; }
    }
}
