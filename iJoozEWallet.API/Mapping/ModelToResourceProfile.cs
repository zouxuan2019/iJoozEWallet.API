using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Resources;

namespace iJoozEWallet.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<EWallet, EWalletResource>();
            CreateMap<TopUpHistory, TopUpResource>();
        }
    }
}