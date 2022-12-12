using AutoMapper;
using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain.Entities;
using BankAccount.Shared.Utilities;
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

namespace BankAccount.Test
{
    [TestFixture]
    public class PotentialMemberWorkflowServiceUnitTests
    {
        #region Fields

        private IMapper _mapper;
        private Mock<ILogger<PotentialMemberWorkflowService>> _loggerMock;
        private Mock<IRepository<PotentialMember>> _potentialMemberRepositoryMock;
        private PotentialMemberWorkflowService _potentialMemberWorkflowService;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            _potentialMemberRepositoryMock = new Mock<IRepository<PotentialMember>>();
            _loggerMock = new Mock<ILogger<PotentialMemberWorkflowService>>();
            _mapper = AutoMapperConfiguration.ConfigureMappings();
            _potentialMemberWorkflowService = new PotentialMemberWorkflowService(_loggerMock.Object, _potentialMemberRepositoryMock.Object, _mapper);
        }

        #endregion

        #region MockData

        private const string Url = "www.test.com";
        private const string IpAddress = "www.test.com";
        private const string FutureDate = "2122-09-10";
        private const string PastDate = "1952-09-10";

        #endregion

        #region Tests

        [TestCase("", "", PastDate)]
        [TestCase(Url, "", PastDate)]
        [TestCase(Url, IpAddress, FutureDate)]
        public void ValidateMetadata_Should_Fail_When_Data_Is_Invalid(string websiteStartingUrl, string ipAddress, string initializationTime)
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new PotentialMemberPayload(websiteStartingUrl, ipAddress, DateTime.Parse(initializationTime)));

            // Act
            var actual = _potentialMemberWorkflowService.ValidateMetadata(model);

            // Assert
            Assert.False(actual.Successful);
            Assert.NotNull(actual.Errors);
            Assert.IsNotEmpty(actual.Errors);
        }

        [Test]
        public void ValidateMetadata_Should_Fail_When_Metadata_Format_Is_Invalid()
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<PotentialMemberPayload>());

            // Act
            var actual = _potentialMemberWorkflowService.ValidateMetadata(model);

            // Assert
            Assert.False(actual.Successful);
            Assert.NotNull(actual.Errors);
            Assert.IsNotEmpty(actual.Errors);
        }

        [Test]
        public void ValidateMetadata_Should_Pass_When_Data_Is_Valid()
        {
            // Arrange
            var model = new PotentialMemberPayload(Url, IpAddress, DateTime.UtcNow);

            // Act
            var actual = _potentialMemberWorkflowService.ValidateMetadata(JsonConvert.SerializeObject(model));

            // Assert
            Assert.True(actual.Successful);
            Assert.Null(actual.Errors);
            Assert.AreEqual(actual.Result.WebsiteStartingUrl, model.WebsiteStartingUrl);
            Assert.AreEqual(actual.Result.IpAddress, model.IpAddress);
            Assert.AreEqual(actual.Result.InitializationTime, model.InitializationTime);
        }

        [TestCase("", "", "")]
        [TestCase(Url, "", "")]
        [TestCase(Url, IpAddress, "")]
        [TestCase(Url, IpAddress, FutureDate)]
        public async Task ExecuteAsync_Should_Fail_When_Metadata_Format_Is_Invalid(string websiteStartingUrl, string ipAddress, string initializationTime)
        {
            // Arrange
            var model = JsonConvert.SerializeObject(new List<PotentialMemberPayload>());

            // Act
            await _potentialMemberWorkflowService.ExecuteAsync(model, Guid.NewGuid().ToString());

            // Assert
            _potentialMemberRepositoryMock.Verify(x => x.SaveOrUpdateAsync(It.IsAny<PotentialMember>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Pass_When_Data_Is_Valid()
        {
            // Arrange
            var model = new PotentialMemberPayload(Url, IpAddress, DateTime.UtcNow);

            // Act
            await _potentialMemberWorkflowService.ExecuteAsync(JsonConvert.SerializeObject(model), Guid.NewGuid().ToString());

            // Assert
            _potentialMemberRepositoryMock.Verify(x => x.SaveOrUpdateAsync(It.IsAny<PotentialMember>()), Times.Once);
        }

        [Test]
        public void WorkFlow_Should_Be_Valid()
        {
            // Assert
            Assert.AreEqual(WorkFlow.PotentialMember, _potentialMemberWorkflowService.WorkFlow);
        }

        #endregion
    }
}
