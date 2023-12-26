using StraddleCore.Models.DTO.Wallets;
using StraddleData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Wallets.Interfaces
{
    public interface IWalletAccountService
    {
        /// <summary>
        /// Create a customer and wallet account.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> CreateWalletAccountAsync();

        /// <summary>
        /// Get customer details using phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<ServiceResponse<WalletCustomerDTO>> GetCustomerByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Verify source or sender's wallet account using account number and customer id.
        /// </summary>
        /// <param name="customerDTO"></param>
        /// <returns></returns>
        Task<ServiceResponse<WalletAccountDTO>> VerifySourceWalletAccountAsync(WalletCustomerDTO customerDTO);

        /// <summary>
        /// Verify destination wallet account using account number.
        /// </summary>
        /// <param name="destinationAccountNumber"></param>
        /// <returns></returns>
        Task<ServiceResponse<WalletAccountDTO>> VerifyDestinationWalletAccountAsync(string destinationAccountNumber);
    }
}
