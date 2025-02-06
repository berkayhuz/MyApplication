using Forum.Application.Common.DTOs;
using MediatR;

namespace Forum.Application.Queries.UserQueries
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public string Username { get; set; }
        public GetUserQuery(string userName)
        {
            Username = userName;
        }
    }
}
