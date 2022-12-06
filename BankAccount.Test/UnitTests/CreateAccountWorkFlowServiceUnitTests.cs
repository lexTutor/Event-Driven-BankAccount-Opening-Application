using AutoMapper;
using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain.Entities;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.Utilities;
using BankAccount.Shared.WorkFlowServices;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Test.UnitTests
{
    [TestFixture]
    public class CreateAccountWorkFlowServiceUnitTests
    {
        #region Fields

        private IMapper _mapper;
        private Mock<ILogger<CreateAccountWorkFlowService>> _loggerMock;
        private Mock<IRepository<Account>> _accountRepositoryMock;
        private Mock<IRepository<CreditScore>> _creditScoreRepositoryMock;
        private Mock<IReferenceNumberService> _referenceNumberServiceMock;
        private Mock<IQueueService> _queueServiceMock;
        private CreateAccountWorkFlowService _createAccountWorkFlowService;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            _accountRepositoryMock = new Mock<IRepository<Account>>();
            _accountRepositoryMock.Setup(x => x.Table).Returns(new List<Account>().AsQueryable().BuildMock());

            _creditScoreRepositoryMock = new Mock<IRepository<CreditScore>>();
            _creditScoreRepositoryMock.Setup(x => x.TableNoTracking).Returns(new List<CreditScore>().AsQueryable().BuildMock());

            _loggerMock = new Mock<ILogger<CreateAccountWorkFlowService>>();

            _queueServiceMock = new Mock<IQueueService>();

            _referenceNumberServiceMock = new Mock<IReferenceNumberService>();
            _referenceNumberServiceMock.Setup(x => x.IncreamentNextValAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((1, 1));
            _mapper = AutoMapperConfiguration.ConfigureMappings();

            _createAccountWorkFlowService = new CreateAccountWorkFlowService(_mapper, _loggerMock.Object, _accountRepositoryMock.Object,
                _creditScoreRepositoryMock.Object, _queueServiceMock.Object, _referenceNumberServiceMock.Object);
        }

        #endregion

        #region MockData

        private const string ValidEmail = "Jane.Doe@optimusinfo.com";
        private const string InValidEmail1 = "Jane.Doeoptimusinfo.com";
        private const string InValidEmail2 = "Jane.Doe@optimusinfocom";
        private const string FirstName = "Jane";
        private const string LastName = "Doe";

        #endregion

        #region Tests

        [TestCase("", "", "")]
        [TestCase(ValidEmail, "", "")]
        [TestCase(InValidEmail1, FirstName, LastName)]
        [TestCase(InValidEmail2, FirstName, LastName)]
        public void ValidateMetadata_Should_Fail_When_Data_Is_Invalid(string email, string firstName, string lastName)
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new CreateAccountPayload(email, firstName, lastName));

            // Act
            var actual = _createAccountWorkFlowService.ValidateMetadata(model);

            // Assert
            Assert.False(actual.Successful);
            Assert.NotNull(actual.Errors);
            Assert.IsNotEmpty(actual.Errors);
        }

        [Test]
        public void ValidateMetadata_Should_Fail_When_Metadata_Format_Is_Invalid()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<CreateAccountWorkFlowService>());

            // Act
            var actual = _createAccountWorkFlowService.ValidateMetadata(model);

            // Assert
            Assert.False(actual.Successful);
            Assert.NotNull(actual.Errors);
            Assert.IsNotEmpty(actual.Errors);
        }

        [Test]
        public void ValidateMetadata_Should_Pass_When_Data_Is_Valid()
        {
            // Arrange
            var model = new CreateAccountPayload(ValidEmail, FirstName, LastName);

            // Act
            var actual = _createAccountWorkFlowService.ValidateMetadata(JsonConvert.SerializeObject(model));

            // Assert
            Assert.True(actual.Successful);
            Assert.Null(actual.Errors);
            Assert.AreEqual(actual.Result.Email, model.Email);
            Assert.AreEqual(actual.Result.FirstName, model.FirstName);
            Assert.AreEqual(actual.Result.LastName, model.LastName);
        }

        [TestCase("", "", "")]
        [TestCase(ValidEmail, "", "")]
        [TestCase(InValidEmail1, FirstName, LastName)]
        [TestCase(InValidEmail2, FirstName, LastName)]
        public async Task ExecuteAsync_Should_Fail_When_Data_Is_Invalid(string email, string firstName, string lastName)
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new CreateAccountPayload(email, firstName, lastName));

            // Act
            await _createAccountWorkFlowService.ExecuteAsync(model, Guid.NewGuid().ToString());

            // Assert
            _accountRepositoryMock.Verify(x => x.Table, Times.Never);
            _creditScoreRepositoryMock.Verify(x => x.TableNoTracking, Times.Never);
            _accountRepositoryMock.Verify(x => x.SaveOrUpdateAsync(It.IsAny<Account>()), Times.Never);
            _queueServiceMock.Verify(x => x.PublishMessageToQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _referenceNumberServiceMock.Verify(x => x.IncreamentNextValAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Pass_When_Data_Is_Valid()
        {
            // Arrange
            CreateAccountPayload model = new CreateAccountPayload(ValidEmail, FirstName, LastName);
            var sessionId = Guid.NewGuid().ToString();
            var metadata = JsonConvert.SerializeObject(model);

            // Act
            await _createAccountWorkFlowService.ExecuteAsync(metadata, sessionId);

            // Assert
            _accountRepositoryMock.Verify(x => x.Table, Times.Once);
            _creditScoreRepositoryMock.Verify(x => x.TableNoTracking, Times.Once);
            _accountRepositoryMock.Verify(x => x.SaveOrUpdateAsync(It.IsAny<Account>()), Times.Once);
            _queueServiceMock.Verify(x => x.PublishMessageToQueue(WorkFlow.CommunicateWithMember.ToString(), It.IsAny<string>(), sessionId), Times.Once);
            _referenceNumberServiceMock.Verify(x => x.IncreamentNextValAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void WorkFlow_Should_Be_Valid()
        {
            // Assert
            Assert.AreEqual(WorkFlow.CreateAccount, _createAccountWorkFlowService.WorkFlow);
        }

        #endregion
    }
}
