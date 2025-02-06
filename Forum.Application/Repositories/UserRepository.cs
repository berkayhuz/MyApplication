using AutoMapper;
using Forum.Application.Aggregates;
using Forum.Application.Interfaces;
using Forum.Domain.Entities.User;
using Forum.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Forum.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly MemberDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(MemberDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UserAggregate> GetByAsync(Expression<Func<User, bool>> predicate)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(predicate);

            return user == null ? null : _mapper.Map<UserAggregate>(user);
        }

        public async Task<bool> ExistsByAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users
                .AnyAsync(predicate);
        }

        public async Task<Result> AddAsync(UserAggregate user)
        {
            if (user == null)
            {
                return Result.Failure("User cannot be null.");
            }

            var userEntity = _mapper.Map<User>(user);
            await _context.Users.AddAsync(userEntity);

            await _context.SaveChangesAsync();
            return Result.Success("User added successfully.");
        }
        public async Task<Result> UpdateAsync(UserAggregate user)
        {
            if (user == null)
            {
                return Result.Failure("User cannot be null.");
            }

            var userEntity = _mapper.Map<User>(user);

            var existingUser = await _context.Users.FindAsync(userEntity.Id);
            if (existingUser != null)
            {
                _context.Entry(existingUser).CurrentValues.SetValues(userEntity);
            }
            else
            {
                _context.Users.Attach(userEntity);
            }
            await _context.SaveChangesAsync();
            return Result.Success("User updated successfully.");
        }
    }
}
