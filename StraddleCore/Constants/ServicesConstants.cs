using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Constants
{
    public static class ServicesConstants
    {
        #region Common Services

        public const string NumberSeed = "7840923156";
        public const string LetterSeed = "ABCDEFGHJKLMNPQRSTUVWXYZ784923156";
        public const string ApiClientExists = "API credentials exist for this client";

        #endregion Common Services
    }

    public static class ServiceCodes
    {
        public const string Success = "00";
        public const string OtpRequired = "05";
        public const string BadRequest = "89";
        public const string Forbidden = "98";
        public const string Unauthorized = "95";
        public const string ServerError = "99";
    }

    public static class ServiceMessages
    {
        public const string Success = "The operation was successful";
        public const string Failed = "An unhandled error has occurred while processing your request";
        public const string UpdateError = "There was an error carrying out operation";
        public const string MisMatch = "The entity Id does not match the supplied Id";
        public const string EntityIsNull = "Supplied entity is null or supplied list of entities is empty. Check our docs";
        public const string EntityNotFound = "The requested resource was not found. Verify that the supplied Id is correct";
        public const string Incompleted = "Some actions may not have been successfully processed";
        public const string EntityExist = "An entity of the same name or id exists";
        public const string InvalidParam = "A supplied parameter or model is invalid. Check the docs";
        public const string UnprocessableEntity = "The action cannot be processed";
        public const string InternalServerError = "An internal server error and request could not processed";
        public const string OperationFailed = "Operation failed";
        public const string ParameterEmptyOrNull = "The parameter list is null or empty";
        public const string RequestIdRequired = "Request Id is required";
    }

    public static class WalletAccountServiceConstants
    {
        public const string CustomerNotFound = "The customer cannot be found";
        public const string AccountNotFound = "The account was not found for this customer";
        public const string AccountMisMatch = "The account does not belong to this customer";
        public const string AccountNotActive = "The account belongs to this customer but it is not active";
        public const string AccountActive = "The account belongs to this customer and it is active";
    }

    public static class WalletTransactionServiceConstants
    {
        public const string TransactionExists = "The transaction already exists";
        public const string InsufficientFunds = "Customer cannot carry out this transaction due to insufficient funds";
        public const string TransactionNotFound = "The transaction was not found";
        public const string TransactionCompleted = "The transaction is already processed or refunded";
    }
}
