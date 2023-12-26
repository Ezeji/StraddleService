using StraddleCore.Models.DTO.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Wallets.Interfaces
{
    public interface IWalletTransactionService
    {
        /// <summary>
        /// Create wallet transaction.
        /// </summary>
        /// <param name="transactionReference"></param>
        /// <param name="createDTO"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> CreateWalletTransactionAsync(string transactionReference, WalletTransactionCreateDTO createDTO);

        /// <summary>
        /// Get wallet transaction by transaction reference.
        /// </summary>
        /// <param name="transactionReference"></param>
        /// <returns></returns>
        Task<ServiceResponse<WalletTransactionDTO>> GetWalletTransactionByTransactionReferenceAsync(string transactionReference);

        /// <summary>
        /// Cancel wallet transaction.
        /// </summary>
        /// <param name="transactionReference"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> CancelWalletTransactionAsync(string transactionReference);
    }
}
