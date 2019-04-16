using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Resources;

namespace iJoozEWallet.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<TopUpResource, TopUpHistory>();
            CreateMap<DeductResource, DeductHistory>();
        }
    }
}