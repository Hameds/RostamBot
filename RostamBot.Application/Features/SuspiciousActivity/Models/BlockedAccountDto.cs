using Araye.Code.Cqrs.Application.Interfaces.Mapping;
using AutoMapper;
using RostamBot.Domain.Entities;

namespace RostamBot.Application.Features.SuspiciousActivity.Models
{
    public class BlockedAccountDto : IHaveCustomMapping
    {
        public long TwitterUserId { get; set; }

        public string TwitterScreenName { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<SuspiciousAccount, BlockedAccountDto>()
          .ForMember(cDTO => cDTO.TwitterUserId, opt => opt.MapFrom(c => c.TwitterUserId))
          .ForMember(cDTO => cDTO.TwitterScreenName, opt => opt.MapFrom(c => c.TwitterScreenName));
        }

    }
}
