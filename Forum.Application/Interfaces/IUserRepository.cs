using Forum.Application.Aggregates;
using Forum.Domain.Entities.User;
using Forum.Domain.Models;
using System.Linq.Expressions;

namespace Forum.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserAggregate> GetByAsync(Expression<Func<User, bool>> predicate);
        Task<bool> ExistsByAsync(Expression<Func<User, bool>> predicate);
        Task<Result> AddAsync(UserAggregate user);
        Task<Result> UpdateAsync(UserAggregate user);
    }
}
