using AutoMapper;
using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain;
using BankAccount.Shared.Domain.Entities;
using BankAccount.Shared.OrchestorService;
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
using System.Net;
using System.Threading.Tasks;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Test.UnitTests
{
    [TestFixture]
    public class OrchestratorServiceUnitTests
    {
        #region Fields

        private IMapper _mapper;
        private Mock<IWorkflowProviderSelector> _workflowProviderMock;
        private Mock<IQueueService> _queueServiceMock;
        private Mock<ILogger<OrchestratorService>> _loggerMock;
        private OrchestratorService _orchestratorService;

        #region PotentialMember Fields

        private Mock<ILogger<PotentialMemberWorkflowService>> _potentialMemberloggerMock;
        private Mock<IRepository<PotentialMember>> _potentialMemberRepositoryMock;
        private PotentialMemberWorkflowService _potentialMemberWorkflowService;

        #endregion

        #region CreateAccount Fields

        private Mock<ILogger<CreateAccountWorkFlowService>> _createAccountLoggerMock;
        private Mock<IRepository<Account>> _accountRepositoryMock;
        private Mock<IRepository<CreditScore>> _creditScoreRepositoryMock;
        private Mock<IReferenceNumberService> _referenceNumberServiceMock;
        private CreateAccountWorkFlowService _createAccountWorkFlowService;

        #endregion

        #region CommunicateWithMember Fields

        private Mock<ILogger<CommunicateWithMemberWorkflowService>> _communicateWithMemberLoggerMock;
        private Mock<IMailService> _mailServiceMock;

        #endregion

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<OrchestratorService>>();
            _queueServiceMock = new Mock<IQueueService>();
            _mapper = AutoMapperConfiguration.ConfigureMappings();

            #region PotentialMember SetUp

            _potentialMemberRepositoryMock = new Mock<IRepository<PotentialMember>>();
            _potentialMemberloggerMock = new Mock<ILogger<PotentialMemberWorkflowService>>();

            #endregion

            #region CreateAccount SetUp

            _createAccountLoggerMock = new Mock<ILogger<CreateAccountWorkFlowService>>();

            _accountRepositoryMock = new Mock<IRepository<Account>>();
            _accountRepositoryMock.Setup(x => x.Table).Returns(new List<Account>().AsQueryable().BuildMock());

            _creditScoreRepositoryMock = new Mock<IRepository<CreditScore>>();
            _creditScoreRepositoryMock.Setup(x => x.TableNoTracking).Returns(new List<CreditScore>().AsQueryable().BuildMock());

            _referenceNumberServiceMock = new Mock<IReferenceNumberService>();
            _referenceNumberServiceMock.Setup(x => x.IncreamentNextValAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((1, 1));

            #endregion

            #region CommunicateWithMember SetUp

            _communicateWithMemberLoggerMock = new Mock<ILogger<CommunicateWithMemberWorkflowService>>();

            _mailServiceMock = new Mock<IMailService>();
            _mailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<MailRequest>())).Returns(Task.CompletedTask);

            #endregion

            _workflowProviderMock = new Mock<IWorkflowProviderSelector>();

            _workflowProviderMock.Setup(x=>x.GetWorkFlowService(WorkFlow.PotentialMember))
                .Returns(new PotentialMemberWorkflowService(_potentialMemberloggerMock.Object, _potentialMemberRepositoryMock.Object, _mapper));

            _workflowProviderMock.Setup(x => x.GetWorkFlowService(WorkFlow.CreateAccount))
                .Returns(new CreateAccountWorkFlowService(_mapper, _createAccountLoggerMock.Object, _accountRepositoryMock.Object,
                _creditScoreRepositoryMock.Object, _queueServiceMock.Object, _referenceNumberServiceMock.Object));

            _workflowProviderMock.Setup(x => x.GetWorkFlowService(WorkFlow.CommunicateWithMember))
                .Returns(new CommunicateWithMemberWorkflowService(_communicateWithMemberLoggerMock.Object, _mailServiceMock.Object));

            _orchestratorService = new OrchestratorService(_workflowProviderMock.Object, _queueServiceMock.Object, _loggerMock.Object);
        }

        #endregion

        #region MockData

        private const string Url = "www.test.com";
        private const string IpAddress = "www.test.com";

        private const string ValidEmail = "Jane.Doe@optimusinfo.com";
        private const string FirstName = "Jane";
        private const string LastName = "Doe";

        private const string Email = "Jane.Doe@optimusinfo.com";
        private const string AccountNumber = "0000000001";
        private const string FullName = "Jane Doe";

        #endregion

        #region Tests

        [TestCase(-1)]
        [TestCase(10000)]
        public async Task InitiateWorkFlow_Should_Fail_When_WorkflowId_Is_Invalid(int workflowId)
        {
            // Act
            var result = await _orchestratorService.InitiateWorkFlow(new InitiateWorkFlowPayload(workflowId, string.Empty, string.Empty));

            // Assert
            Assert.False(result.Successful);
            Assert.NotNull(result.Errors);
            Assert.IsNotEmpty(result.Errors);
            Assert.Null(result.Result);
        }

        [TestCase((int)WorkFlow.PotentialMember, "")]
        [TestCase((int)WorkFlow.CreateAccount, "")]
        [TestCase((int)WorkFlow.CommunicateWithMember, "")]
        public async Task InitiateWorkFlow_Should_Fail_When_Metdata_Is_Invalid_For_The_Specified_Workflow(int workflowId, string metadata)
        {
            // Act
            var result = await _orchestratorService.InitiateWorkFlow(new InitiateWorkFlowPayload(workflowId, string.Empty, string.Empty));

            // Assert
            Assert.False(result.Successful);
            Assert.NotNull(result.Errors);
            Assert.IsNotEmpty(result.Errors);
            Assert.Null(result.Result);
        }

        [Test]
        public async Task InitiateWorkFlow_Should_Pass_When_Metdata_Is_Valid_For_PotentialMember_Workflow()
        {
            // Arrange
            PotentialMemberPayload model = new(Url, IpAddress, DateTime.UtcNow);
            var metadata = JsonConvert.SerializeObject(model);
            var sessionId = Guid.NewGuid().ToString();

            // Act
            var actual = await _orchestratorService.InitiateWorkFlow(new InitiateWorkFlowPayload((int)WorkFlow.PotentialMember, metadata,sessionId));

            // Assert
            _queueServiceMock.Verify(x => x.PublishMessageToQueue(WorkFlow.PotentialMember.ToString(), metadata, sessionId), Times.Once);

            Assert.True(actual.Successful);
            Assert.Null(actual.Errors);
            Assert.AreEqual(actual.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(actual.Result, sessionId);
        }

        [Test]
        public async Task InitiateWorkFlow_Should_Pass_When_Metdata_Is_Valid_For_CreateAccount_Workflow()
        {
            // Arrange
            CreateAccountPayload model = new(ValidEmail, FirstName, LastName);
            var metadata = JsonConvert.SerializeObject(model);
            var sessionId = Guid.NewGuid().ToString();

            // Act
            var actual = await _orchestratorService.InitiateWorkFlow(new InitiateWorkFlowPayload((int)WorkFlow.CreateAccount, metadata, sessionId));

            // Assert
            _queueServiceMock.Verify(x => x.PublishMessageToQueue(WorkFlow.CreateAccount.ToString(), metadata, sessionId), Times.Once);

            Assert.True(actual.Successful);
            Assert.Null(actual.Errors);
            Assert.AreEqual(actual.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(actual.Result, sessionId);
        }

        [Test]
        public async Task InitiateWorkFlow_Should_Pass_When_Metdata_Is_Valid_For_CommunicateMember_Workflow()
        {
            // Arrange
            CommunicateWithMemberPayload model = new(AccountNumber, 10, Email, FullName);
            var metadata = JsonConvert.SerializeObject(model);
            var sessionId = Guid.NewGuid().ToString();

            // Act
            var actual = await _orchestratorService.InitiateWorkFlow(new InitiateWorkFlowPayload((int)WorkFlow.CommunicateWithMember, metadata, sessionId));

            // Assert
            _queueServiceMock.Verify(x => x.PublishMessageToQueue(WorkFlow.CommunicateWithMember.ToString(), metadata, sessionId), Times.Once);

            Assert.True(actual.Successful);
            Assert.Null(actual.Errors);
            Assert.AreEqual(actual.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(actual.Result, sessionId);
        }

        #endregion
    }
}
