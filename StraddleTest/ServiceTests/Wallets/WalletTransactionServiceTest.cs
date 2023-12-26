using AutoMapper;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using StraddleCore.Configurations.Azure.Interfaces;
using StraddleCore.Constants;
using StraddleCore.Models.DTO.Wallets;
using StraddleCore.Services;
using StraddleCore.Services.Wallets;
using StraddleCore.Services.Wallets.Interfaces;
using StraddleData.Enums;
using StraddleData.Models.Wallets;
using StraddleRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StraddleTest.ServiceTests.Wallets
{
    public class WalletTransactionServiceTest
    {
        private readonly Mock<IGenericRepository<WalletTransaction>> transactionRepoMock = new Mock<IGenericRepository<WalletTransaction>>(MockBehavior.Strict);
        private readonly Mock<IGenericRepository<WalletAccount>> accountRepoMock = new Mock<IGenericRepository<WalletAccount>>(MockBehavior.Strict);
        private readonly Mock<IAzureServiceBusQueueConfiguration> queueConfigurationMock = new Mock<IAzureServiceBusQueueConfiguration>(MockBehavior.Strict);
        private readonly Mock<IWalletAccountService> accountServiceMock = new Mock<IWalletAccountService>(MockBehavior.Strict);

        private readonly IMapper mapper;

        private WalletTransactionService _service;

        public WalletTransactionServiceTest()
        {
            MapperConfiguration configuration = new(x => x.AddProfile(new WalletProfile()));
            mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task CreateWalletTransactionAsync_Should_Return_ParameterEmptyOrNull_If_Parameter_Is_Null()
        {
            //Arrange
            string transactionReference = string.Empty;

            WalletTransactionCreateDTO createDTO = new()
            {
                TransactionAmount = 10
            };

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.ParameterEmptyOrNull;

            //Act
            ServiceResponse<string> actual = await _service.CreateWalletTransactionAsync(transactionReference, createDTO);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CreateWalletTransactionAsync_Should_Return_TransactionExists_If_Transaction_Is_Found()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            WalletTransactionCreateDTO createDTO = new()
            {
                TransactionAmount = 10
            };

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = transactionReference
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletTransactionServiceConstants.TransactionExists;

            //Act
            ServiceResponse<string> actual = await _service.CreateWalletTransactionAsync(transactionReference, createDTO);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CreateWalletTransactionAsync_Should_Return_EntityNotFound_If_Customer_Is_NotFound()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            WalletTransactionCreateDTO createDTO = new()
            {
                TransactionAmount = 10
            };

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = Guid.NewGuid().ToString()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            ServiceResponse<WalletCustomerDTO> customerResponse = new()
            {
                ResponseObject = null!
            };

            accountServiceMock.Setup(x => x.GetCustomerByPhoneNumberAsync(It.IsAny<string>())).ReturnsAsync(customerResponse);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.EntityNotFound;

            //Act
            ServiceResponse<string> actual = await _service.CreateWalletTransactionAsync(transactionReference, createDTO);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CreateWalletTransactionAsync_Should_Return_AccountNotFound_If_SourceAccountNumber_Is_NotFound()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            WalletTransactionCreateDTO createDTO = new()
            {
                TransactionAmount = 10,
                SourceAccountNumber = "224466885591"
            };

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = Guid.NewGuid().ToString()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            ServiceResponse<WalletCustomerDTO> customerResponse = new()
            {
                ResponseObject = new WalletCustomerDTO()
            };

            accountServiceMock.Setup(x => x.GetCustomerByPhoneNumberAsync(It.IsAny<string>())).ReturnsAsync(customerResponse);

            List<WalletAccount> accounts = new()
            {
                new WalletAccount()
                {
                    BankAccountNumber = "442288669155"
                }
            };

            IQueryable<WalletAccount> accountQuery = accounts.AsQueryable().BuildMock();

            accountRepoMock.Setup(x => x.Query()).Returns(accountQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletAccountServiceConstants.AccountNotFound;

            //Act
            ServiceResponse<string> actual = await _service.CreateWalletTransactionAsync(transactionReference, createDTO);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CreateWalletTransactionAsync_Should_Return_InsufficientFunds_If_Account_Has_Insufficient_Funds()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();
            string sourceAccountNumber = "224466885591";

            WalletTransactionCreateDTO createDTO = new()
            {
                TransactionAmount = 10000,
                SourceAccountNumber = sourceAccountNumber
            };

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = Guid.NewGuid().ToString()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            ServiceResponse<WalletCustomerDTO> customerResponse = new()
            {
                ResponseObject = new WalletCustomerDTO()
            };

            accountServiceMock.Setup(x => x.GetCustomerByPhoneNumberAsync(It.IsAny<string>())).ReturnsAsync(customerResponse);

            List<WalletAccount> accounts = new()
            {
                new WalletAccount()
                {
                    BankAccountNumber = sourceAccountNumber,
                    WalletBalance = 1000,
                    LienAmount = 500
                }
            };

            IQueryable<WalletAccount> accountQuery = accounts.AsQueryable().BuildMock();

            accountRepoMock.Setup(x => x.Query()).Returns(accountQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletTransactionServiceConstants.InsufficientFunds;

            //Act
            ServiceResponse<string> actual = await _service.CreateWalletTransactionAsync(transactionReference, createDTO);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CreateWalletTransactionAsync_Should_Return_Success_If_Account_Has_Sufficient_Funds()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();
            string sourceAccountNumber = "224466885591";

            WalletTransactionCreateDTO createDTO = new()
            {
                TransactionAmount = 100,
                SourceAccountNumber = sourceAccountNumber
            };

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = Guid.NewGuid().ToString()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            ServiceResponse<WalletCustomerDTO> customerResponse = new()
            {
                ResponseObject = new WalletCustomerDTO()
            };

            accountServiceMock.Setup(x => x.GetCustomerByPhoneNumberAsync(It.IsAny<string>())).ReturnsAsync(customerResponse);

            List<WalletAccount> accounts = new()
            {
                new WalletAccount()
                {
                    BankAccountNumber = sourceAccountNumber,
                    WalletBalance = 10000,
                    LienAmount = 500
                }
            };

            IQueryable<WalletAccount> accountQuery = accounts.AsQueryable().BuildMock();

            accountRepoMock.Setup(x => x.Query()).Returns(accountQuery);

            accountRepoMock.Setup(x => x.SaveChangesToDbAsync()).ReturnsAsync(1);

            transactionRepoMock.Setup(x => x.CreateAsync(It.IsAny<WalletTransaction>(), It.IsAny<bool>())).ReturnsAsync(1);

            queueConfigurationMock.Setup(x => x.SendMessageAsync(It.IsAny<string>())).ReturnsAsync(Task.CompletedTask);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.Success;

            //Act
            ServiceResponse<string> actual = await _service.CreateWalletTransactionAsync(transactionReference, createDTO);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task GetWalletTransactionByTransactionReferenceAsync_Should_Return_ParameterEmptyOrNull_If_Parameter_Is_Null()
        {
            //Arrange
            string transactionReference = string.Empty;

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.ParameterEmptyOrNull;

            //Act
            ServiceResponse<WalletTransactionDTO> actual = await _service.GetWalletTransactionByTransactionReferenceAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task GetWalletTransactionByTransactionReferenceAsync_Should_Return_TransactionNotFound_If_TransactionReference_Is_NotFound()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = Guid.NewGuid().ToString()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletTransactionServiceConstants.TransactionNotFound;

            //Act
            ServiceResponse<WalletTransactionDTO> actual = await _service.GetWalletTransactionByTransactionReferenceAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task GetWalletTransactionByTransactionReferenceAsync_Should_Return_Success_If_TransactionReference_Is_Found()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = transactionReference,
                    TransactionDetails = JsonConvert.SerializeObject(new WalletTransactionCreateDTO())
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.Success;

            //Act
            ServiceResponse<WalletTransactionDTO> actual = await _service.GetWalletTransactionByTransactionReferenceAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CancelWalletTransactionAsync_Should_Return_ParameterEmptyOrNull_If_Parameter_Is_Null()
        {
            //Arrange
            string transactionReference = string.Empty;

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.ParameterEmptyOrNull;

            //Act
            ServiceResponse<string> actual = await _service.CancelWalletTransactionAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CancelWalletTransactionAsync_Should_Return_TransactionNotFound_If_TransactionReference_Is_NotFound()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = Guid.NewGuid().ToString()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletTransactionServiceConstants.TransactionNotFound;

            //Act
            ServiceResponse<string> actual = await _service.CancelWalletTransactionAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CancelWalletTransactionAsync_Should_Return_TransactionCompleted_If_TransactionStatus_Is_ProcessedOrRefunded()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = transactionReference,
                    TransactionStatus = (int)TransactionStatus.Processed
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletTransactionServiceConstants.TransactionCompleted;

            //Act
            ServiceResponse<string> actual = await _service.CancelWalletTransactionAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CancelWalletTransactionAsync_Should_Return_AccountNotFound_If_TransactionStatus_Is_Pending_And_SourceAccountId_Is_NotFound()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = transactionReference,
                    TransactionStatus = (int)TransactionStatus.Pending,
                    SourceAccountId = Guid.NewGuid()
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            List<WalletAccount> accounts = new()
            {
                new WalletAccount()
                {
                    AccountId = Guid.NewGuid()
                }
            };

            IQueryable<WalletAccount> accountQuery = accounts.AsQueryable().BuildMock();

            accountRepoMock.Setup(x => x.Query()).Returns(accountQuery);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = WalletAccountServiceConstants.AccountNotFound;

            //Act
            ServiceResponse<string> actual = await _service.CancelWalletTransactionAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }

        [Fact]
        public async Task CancelWalletTransactionAsync_Should_Return_Success_If_TransactionStatus_Is_Pending_And_SourceAccountId_Is_Found()
        {
            //Arrange
            string transactionReference = Guid.NewGuid().ToString();
            Guid sourceAccountId = Guid.NewGuid();

            List<WalletTransaction> transactions = new()
            {
                new WalletTransaction()
                {
                    TransactionReference = transactionReference,
                    TransactionStatus = (int)TransactionStatus.Pending,
                    SourceAccountId = sourceAccountId,
                    TransactionAmount = 10
                }
            };

            IQueryable<WalletTransaction> transactionQuery = transactions.AsQueryable().BuildMock();

            transactionRepoMock.Setup(x => x.Query()).Returns(transactionQuery);

            List<WalletAccount> accounts = new()
            {
                new WalletAccount()
                {
                    AccountId = sourceAccountId,
                    LienAmount = 410
                }
            };

            IQueryable<WalletAccount> accountQuery = accounts.AsQueryable().BuildMock();

            accountRepoMock.Setup(x => x.Query()).Returns(accountQuery);

            accountRepoMock.Setup(x => x.SaveChangesToDbAsync()).ReturnsAsync(1);

            transactionRepoMock.Setup(x => x.SaveChangesToDbAsync()).ReturnsAsync(1);

            _service = new WalletTransactionService(mapper, queueConfigurationMock.Object, accountServiceMock.Object,
                transactionRepoMock.Object, accountRepoMock.Object);

            string expected = ServiceMessages.Success;

            //Act
            ServiceResponse<string> actual = await _service.CancelWalletTransactionAsync(transactionReference);

            //Assert
            Assert.Equal(expected, actual.StatusMessage);
        }
    }
}
