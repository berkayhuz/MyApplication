using Forum.Application.Common.DTOs;
using MediatR;

namespace Forum.Application.Queries.UserQueries
{
    public class GetUserProfileQuery : IRequest<UserProfileDto>
    {
        public string Username { get; set; }

        public GetUserProfileQuery(string username)
        {
            Username = username;
        }
    }
}
