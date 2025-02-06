using Forum.Domain.Models;
using MediatR;

namespace Forum.Application.Requests.Member
{
    public class CreateSocialLinksRequest
    {
        public string Name { get; set; }
    }
}
