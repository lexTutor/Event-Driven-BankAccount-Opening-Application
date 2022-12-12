using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain;
using BankAccount.Shared.WorkFlowServices;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Test.UnitTests
{
    [TestFixture]
    public class CommunicateWithMemberWorkflowServiceUnitTests
    {

        #region Fields

        private Mock<ILogger<CommunicateWithMemberWorkflowService>> _loggerMock;
        private Mock<IMailService> _mailServiceMock;
        private CommunicateWithMemberWorkflowService _communicateWithMemberWorkflowService;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<CommunicateWithMemberWorkflowService>>();

            _mailServiceMock = new Mock<IMailService>();
            _mailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<MailRequest>())).Returns(Task.CompletedTask);

            _communicateWithMemberWorkflowService = new CommunicateWithMemberWorkflowService(_loggerMock.Object, _mailServiceMock.Object);
        }

        #endregion

        #region MockData

        private const string Email = "Jane.Doe@optimusinfo.com";
        private const string AccountNumber = "0000000001";
        private const string FullName = "Jane Doe";

        #endregion

        #region Tests

        [TestCase("", "", "")]
        [TestCase(Email, "", "")]
        [TestCase(Email, AccountNumber, "")]
        [TestCase(Email, AccountNumber, null)]
        public void ValidateMetadata_Should_Fail_When_Data_Is_Invalid(string email, string accountNumber, string fullName)
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new CommunicateWithMemberPayload(accountNumber, 10, email, fullName));

            // Act
            var actual = _communicateWithMemberWorkflowService.ValidateMetadata(model);

            // Assert
            Assert.False(actual.Successful);
            Assert.NotNull(actual.Errors);
            Assert.IsNotEmpty(actual.Errors);
        }

        [Test]
        public void ValidateMetadata_Should_Fail_When_Metadata_Format_Is_Invalid()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<CommunicateWithMemberPayload>());

            // Act
            var actual = _communicateWithMemberWorkflowService.ValidateMetadata(model);

            // Assert
            Assert.False(actual.Successful);
            Assert.NotNull(actual.Errors);
            Assert.IsNotEmpty(actual.Errors);
        }

        [Test]
        public void ValidateMetadata_Should_Pass_When_Data_Is_Valid()
        {
            // Arrange
            var model = new CommunicateWithMemberPayload(AccountNumber,10, Email, FullName);

            // Act
            var actual = _communicateWithMemberWorkflowService.ValidateMetadata(JsonConvert.SerializeObject(model));

            // Assert
            Assert.True(actual.Successful);
            Assert.Null(actual.Errors);
            Assert.AreEqual(actual.Result.Email, model.Email);
            Assert.AreEqual(actual.Result.AccountNumber, model.AccountNumber);
            Assert.AreEqual(actual.Result.FullName, model.FullName);
            Assert.AreEqual(actual.Result.CreditScore, model.CreditScore);
        }

        [TestCase("", "", "")]
        [TestCase(Email, "", "")]
        [TestCase(Email, AccountNumber, "")]
        [TestCase(Email, AccountNumber, null)]
        public async Task ExecuteAsync_Should_Fail_When_Data_Is_Invalid(string email, string accountNumber, string fullName)
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new CommunicateWithMemberPayload(accountNumber, 10, email, fullName));

            // Act
            await _communicateWithMemberWorkflowService.ExecuteAsync(model, Guid.NewGuid().ToString());

            // Assert
            _mailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Pass_When_Data_Is_Valid()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new CommunicateWithMemberPayload(AccountNumber, 10, Email, FullName));

            // Act
            await _communicateWithMemberWorkflowService.ExecuteAsync(model, Guid.NewGuid().ToString());

            // Assert
            _mailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<MailRequest>()), Times.Once);
        }

        [Test]
        public void WorkFlow_Should_Be_Valid()
        {
            // Assert
            Assert.AreEqual(WorkFlow.CommunicateWithMember, _communicateWithMemberWorkflowService.WorkFlow);
        }

        #endregion
    }
}
