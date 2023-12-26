using AutoMapper;
using StraddleCore.Models.DTO.Partners;
using StraddleCore.Models.DTO.Wallets;
using StraddleData.Models.Partners;
using StraddleData.Models.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Partners
{
    public class PartnerProfile : Profile
    {
        public PartnerProfile() 
        {
            CreateMap<ApiClient, ApiClientDTO>();
        }
    }
}
