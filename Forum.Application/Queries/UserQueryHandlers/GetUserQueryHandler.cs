using Forum.Application.Common.DTOs;
using Forum.Application.Interfaces;
using Forum.Application.Queries.UserQueries;
using MediatR;

namespace Forum.Application.Queries.UserQueryHandlers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByAsync(u => u.UserName == request.Username);

            if (user == null)
            {
                return null;
            }
            return new UserDto { Id = user.Id,
                                 FirstName = user.FirstName, 
                                 LastName = user.LastName, 
                                 Username = user.Username, 
                                 Email = user.Email.ToString() };
        }
    }
}