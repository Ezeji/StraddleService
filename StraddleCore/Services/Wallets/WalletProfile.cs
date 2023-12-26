using AutoMapper;
using StraddleCore.Models.DTO.Wallets;
using StraddleData.Enums;
using StraddleData.Models.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Wallets
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<WalletCustomer, WalletCustomerDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (CustomerStatus)src.Status))
                .ForMember(dest => dest.IdentificationType, opt => opt.MapFrom(src => (IdentityType)src.IdentificationType))
                .ForMember(dest => dest.BankAccountNumber, opt => opt.MapFrom(src => src.WalletAccounts.FirstOrDefault().BankAccountNumber));

            CreateMap<WalletAccount, WalletAccountDTO>()
                .ForMember(dest => dest.BankAccountType, opt => opt.MapFrom(src => (AccountType)src.BankAccountType))
                .ForMember(dest => dest.AccountStatus, opt => opt.MapFrom(src => (AccountStatus)src.AccountStatus))
                .ForMember(dest => dest.BankAccountCurrency, opt => opt.MapFrom(src => (AccountCurrency)src.BankAccountCurrency))
                .ForMember(dest => dest.BankAccountTierLevel, opt => opt.MapFrom(src => (TierLevel)src.BankAccountTierLevel));

            CreateMap<WalletTransactionCreateDTO, WalletTransaction>();

            CreateMap<WalletTransactionCreateDTO, WalletTransactionDTO>();
        }
    }
}
