using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StraddleCore.Configurations.Azure.Interfaces;
using StraddleCore.Constants;
using StraddleCore.Models.DTO.Shared;
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
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IMapper _mapper;
        private readonly IAzureServiceBusQueueConfiguration _queueConfiguration;
        private readonly IWalletAccountService _accountService;
        private readonly IGenericRepository<WalletTransaction> _transactionRepo;
        private readonly IGenericRepository<WalletAccount> _accountRepo;

        public WalletTransactionService(IMapper mapper, IAzureServiceBusQueueConfiguration queueConfiguration, 
            IWalletAccountService accountService, IGenericRepository<WalletTransaction> transactionRepo, 
            IGenericRepository<WalletAccount> accountRepo)
        {
            _mapper = mapper;
            _queueConfiguration = queueConfiguration;
            _accountService = accountService;
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
        }

        public async Task<ServiceResponse<string>> CreateWalletTransactionAsync(string transactionReference, WalletTransactionCreateDTO createDTO)
        {
            if (string.IsNullOrEmpty(transactionReference) || createDTO == null)
            {
                return ServiceResponse<string>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            //check if transaction exists
            bool transactionExists = _transactionRepo.Query()
                .Any(transaction => transaction.TransactionReference == transactionReference);

            if (transactionExists)
            {
                return ServiceResponse<string>.Failed(WalletTransactionServiceConstants.TransactionExists);
            }

            //get customer and account profile
            ServiceResponse<WalletCustomerDTO> customerResponse = await _accountService.GetCustomerByPhoneNumberAsync(createDTO.SourcePhoneNumber!);

            if (customerResponse.ResponseObject == null)
            {
                return ServiceResponse<string>.Failed(ServiceMessages.EntityNotFound);
            }

            //verify source account
            WalletAccount? accountData = await _accountRepo.Query()
                                                           .FirstOrDefaultAsync(account => account.BankAccountNumber == createDTO.SourceAccountNumber);

            if (accountData == null)
            {
                return ServiceResponse<string>.Failed(WalletAccountServiceConstants.AccountNotFound);
            }

            //TODO: Implement verification of destination account

            //TODO: Implement transaction and daily limits check

            //check if customer has sufficient funds to transact
            if (createDTO.TransactionAmount > (accountData.WalletBalance - accountData.LienAmount))
            {
                return ServiceResponse<string>.Failed(WalletTransactionServiceConstants.InsufficientFunds);
            }

            //lien transaction amount
            decimal newLienAmount = accountData.LienAmount + createDTO.TransactionAmount;

            accountData.LienAmount = newLienAmount;
            accountData.DateUpdated = DateTime.UtcNow;

            await _accountRepo.SaveChangesToDbAsync();

            //map dto to model
            WalletTransaction walletTransaction = _mapper.Map<WalletTransaction>(createDTO);
            walletTransaction.SourceAccountId = accountData.AccountId;
            walletTransaction.TransactionReference = string.Empty;
            walletTransaction.TransactionDetails = JsonConvert.SerializeObject(createDTO);

            int result = await _transactionRepo.CreateAsync(walletTransaction);

            if (result == 1)
            {
                //publish disburse transaction message to queue
                DisburseTransactionDTO disburseTransactionDTO = new()
                {
                    TransactionReference = transactionReference,
                    SourceAccountId = accountData.AccountId
                };

                string message = JsonConvert.SerializeObject(disburseTransactionDTO);

                await _queueConfiguration.SendMessageAsync(message);
            }

            return ServiceResponse<string>.Success(string.Empty, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<WalletTransactionDTO>> GetWalletTransactionByTransactionReferenceAsync(string transactionReference)
        {
            if (string.IsNullOrEmpty(transactionReference))
            {
                return ServiceResponse<WalletTransactionDTO>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            WalletTransaction? transactionData = await _transactionRepo.Query()
                                                                       .FirstOrDefaultAsync(transaction => transaction.TransactionReference == transactionReference);

            if (transactionData == null)
            {
                return ServiceResponse<WalletTransactionDTO>.Failed(WalletTransactionServiceConstants.TransactionNotFound);
            }

            WalletTransactionCreateDTO createDTO = JsonConvert.DeserializeObject<WalletTransactionCreateDTO>(transactionData.TransactionDetails);

            WalletTransactionDTO transactionDTO = _mapper.Map<WalletTransactionDTO>(createDTO);
            transactionDTO.TransactionDate = transactionData.DateCreated;
            transactionDTO.ConvertedTransactionAmount = transactionData.ConvertedTransactionAmount;
            transactionDTO.ExchangeRate = transactionData.ExchangeRate;
            transactionDTO.GrossTransactionAmount = transactionData.GrossTransactionAmount;
            transactionDTO.TransactionFee = transactionData.TransactionFee;
            transactionDTO.TransactionStatus = (TransactionStatus)transactionData.TransactionStatus;

            return ServiceResponse<WalletTransactionDTO>.Success(transactionDTO, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<string>> CancelWalletTransactionAsync(string transactionReference)
        {
            if (string.IsNullOrEmpty(transactionReference))
            {
                return ServiceResponse<string>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            WalletTransaction? transactionData = await _transactionRepo.Query()
                                                                       .FirstOrDefaultAsync(transaction => transaction.TransactionReference == transactionReference);

            if (transactionData == null)
            {
                return ServiceResponse<string>.Failed(WalletTransactionServiceConstants.TransactionNotFound);
            }

            if (transactionData.TransactionStatus == (int)TransactionStatus.Processed 
                || transactionData.TransactionStatus == (int)TransactionStatus.Refunded)
            {
                return ServiceResponse<string>.Failed(WalletTransactionServiceConstants.TransactionCompleted);
            }

            WalletAccount? accountData = await _accountRepo.Query()
                                                           .FirstOrDefaultAsync(account => account.AccountId == transactionData.SourceAccountId);

            if (accountData == null)
            {
                return ServiceResponse<string>.Failed(WalletAccountServiceConstants.AccountNotFound);
            }

            //unlien transaction amount
            decimal newLienAmount = accountData.LienAmount - transactionData.TransactionAmount;

            accountData.LienAmount = newLienAmount;
            accountData.DateUpdated = DateTime.UtcNow;

            await _accountRepo.SaveChangesToDbAsync();

            //save to db
            transactionData.TransactionStatus = (int)TransactionStatus.Cancelled;
            transactionData.DateUpdated = DateTime.UtcNow;

            await _transactionRepo.SaveChangesToDbAsync();

            return ServiceResponse<string>.Success(string.Empty, ServiceMessages.Success);
        }

    }
}
