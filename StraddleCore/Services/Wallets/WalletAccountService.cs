using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StraddleCore.Constants;
using StraddleCore.Helpers;
using StraddleCore.Models.DTO.Wallets;
using StraddleCore.Services.Wallets.Interfaces;
using StraddleData.Enums;
using StraddleData.Models.Wallets;
using StraddleRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Wallets
{
    public class WalletAccountService : IWalletAccountService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<WalletAccount> _accountRepo;
        private readonly IGenericRepository<WalletCustomer> _customerRepo;

        public WalletAccountService(IMapper mapper, IGenericRepository<WalletAccount> accountRepo,
            IGenericRepository<WalletCustomer> customerRepo)
        {
            _mapper = mapper;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
        }

        //TODO: To be implemented by linking an account via an external API and further
        //creating a wallet account together with a customer profile for the customer
        public async Task<ServiceResponse<string>> CreateWalletAccountAsync()
        {
            return ServiceResponse<string>.Success(string.Empty, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<WalletCustomerDTO>> GetCustomerByPhoneNumberAsync(string phoneNumber)
        {
            phoneNumber = TextHelpers.FormatPhoneNumber(phoneNumber);

            WalletCustomer? customerData = await _customerRepo.Query()
                .Include(customer => customer.WalletAccounts)
                .FirstOrDefaultAsync(customer => customer.PhoneNumber == phoneNumber);

            if (customerData == null)
            {
                return ServiceResponse<WalletCustomerDTO>.Failed(WalletAccountServiceConstants.CustomerNotFound);
            }

            WalletCustomerDTO customerDto = _mapper.Map<WalletCustomerDTO>(customerData);

            return ServiceResponse<WalletCustomerDTO>.Success(customerDto, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<WalletAccountDTO>> VerifySourceWalletAccountAsync(WalletCustomerDTO customerDTO)
        {
            if (customerDTO == null)
            {
                return ServiceResponse<WalletAccountDTO>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            //verify if the account exists
            WalletAccount? accountData = await _accountRepo.Query()
                .Include(account => account.Customer)
                .FirstOrDefaultAsync(account => account.BankAccountNumber == customerDTO.BankAccountNumber);

            if (accountData == null)
            {
                return ServiceResponse<WalletAccountDTO>.Failed(WalletAccountServiceConstants.AccountNotFound);
            }

            //verify if account belongs to customer
            if (accountData.CustomerId != customerDTO.CustomerId)
            {
                return ServiceResponse<WalletAccountDTO>.Failed(WalletAccountServiceConstants.AccountMisMatch);
            }

            //verify if account is active
            if (accountData.AccountStatus != (int)CustomerStatus.Active)
            {
                return ServiceResponse<WalletAccountDTO>.Failed(WalletAccountServiceConstants.AccountNotActive);
            }

            //if account is active, return account information
            WalletAccountDTO accountDto = _mapper.Map<WalletAccountDTO>(accountData);

            return ServiceResponse<WalletAccountDTO>.Success(accountDto, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<WalletAccountDTO>> VerifyDestinationWalletAccountAsync(string destinationAccountNumber)
        {
            if (string.IsNullOrEmpty(destinationAccountNumber))
            {
                return ServiceResponse<WalletAccountDTO>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            //TODO: if we have more than the current major types of Transfer i.e Inter and Intra, we can add other conditionals.

            //verify if the account exists
            WalletAccount? internalAccountData = await _accountRepo.Query()
                .FirstOrDefaultAsync(account => account.BankAccountNumber == destinationAccountNumber);

            if (internalAccountData == null)
            {
                return ServiceResponse<WalletAccountDTO>.Failed(WalletAccountServiceConstants.AccountNotFound);
            }

            //verify if account is active
            if (internalAccountData.AccountStatus != (int)CustomerStatus.Active)
            {
                return ServiceResponse<WalletAccountDTO>.Failed(WalletAccountServiceConstants.AccountNotActive);
            }

            //map wallet account to dto
            WalletAccountDTO accountDto = _mapper.Map<WalletAccountDTO>(internalAccountData);

            return ServiceResponse<WalletAccountDTO>.Success(accountDto, ServiceMessages.Success);
        }

    }
}
