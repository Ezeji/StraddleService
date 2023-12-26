using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StraddleCore.Configurations.Azure.Interfaces;
using StraddleCore.Constants;
using StraddleCore.Models;
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
        private readonly IConfiguration _configuration;

        public WalletTransactionService(IMapper mapper, IAzureServiceBusQueueConfiguration queueConfiguration, 
            IWalletAccountService accountService, IGenericRepository<WalletTransaction> transactionRepo, 
            IGenericRepository<WalletAccount> accountRepo, IConfiguration configuration)
        {
            _mapper = mapper;
            _queueConfiguration = queueConfiguration;
            _accountService = accountService;
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _configuration = configuration;
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

        public async Task<ServiceResponse<string>> RefundWalletTransactionAsync(string transactionReference)
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

            if (transactionData.TransactionStatus != (int)TransactionStatus.Processed)
            {
                return ServiceResponse<string>.Failed(WalletTransactionServiceConstants.TransactionNotProcessed);
            }

            WalletAccount? accountData = await _accountRepo.Query()
                                                           .FirstOrDefaultAsync(account => account.AccountId == transactionData.SourceAccountId);

            if (accountData == null)
            {
                return ServiceResponse<string>.Failed(WalletAccountServiceConstants.AccountNotFound);
            }

            StraddleConfig straddleConfig = new();
            _configuration.GetSection(StraddleConfig.ConfigName).Bind(straddleConfig);

            if (transactionData.DateCreated.Value.AddDays(straddleConfig.AllowedRefundDuration) <= DateTime.UtcNow)
            {
                return ServiceResponse<string>.Failed(WalletTransactionServiceConstants.RefundDurationElapsed);
            }

            //TODO: Implement initiating of the refund process by calling an external API.
            //A background job can be implemented on StraddleDisburseTransactionService for
            //checking the status of the refund process. Upon completion of the refund, necessary
            //db updates such as for wallet balance, total debits, total ledger debits.etc should be done.
            //If a successful initiation process occurs, should a notification be sent to the customer
            //to inform the customer on how long it takes for a refund process to be completed?
            //If unsuccessful, should a retry occur or will the request be further handled manually?

            return ServiceResponse<string>.Success(string.Empty, ServiceMessages.Success);
        }

    }
}
