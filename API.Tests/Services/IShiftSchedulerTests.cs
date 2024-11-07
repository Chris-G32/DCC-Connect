using API.Services;

using Microsoft.Extensions.Configuration;
using API.Config;
using API.Constants;
using MongoDB.Driver;
using API.Utils;
using API.Models;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using AutoFixture;


namespace API.Tests.Services;
/// <summary>
/// These tests are fairly minimal to avoid testing the internals too much of the mongoClient class itself.
/// </summary>
[TestClass]
public class IShiftSchedulerTests
{
    private Fixture _fixture;
    private Mock<ILogger<ShiftScheduler>> _loggerMock;
    [TestInitialize]
    public void Setup()
    {
        _fixture = new Fixture();
        _loggerMock = new Mock<ILogger<ShiftScheduler>>();
    }
    [TestMethod]
    public void TestAssignShift()
    {
        // Create the UpdateResult instance first
        var result = _fixture.Build<UpdateResult.Acknowledged>()
            .With(res => res.ModifiedCount, 1)
            .Create() ?? throw new Exception("This should not be null");
        
        // Now freeze the created instance
        var res=_fixture.Freeze<UpdateResult>();
        res = result;

        Assert.Equals(_fixture.Create<UpdateResult>(),res);
        var entityRetriever = new Mock<IEntityRetriever>();
        
        var mockAvailabiltyService = new Mock<IAvailabiltyService>();
        mockAvailabiltyService.Setup(serv => serv.IsEmployeeSchedulableForShift(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        var scheduler = new ShiftScheduler(_loggerMock.Object, entityRetriever.Object, mockAvailabiltyService.Object);

        //
        var sut = () => { scheduler.AssignShift(_fixture.Create<ShiftAssignment>()); };
        sut.Should().Throw<Exception>();
    }
    [TestMethod]
    public void ClientIsNotNull()
    {
        // Arrange
        var mockSettings = new Mock<IMongoDBSettingsProvider>();

        mockSettings.Setup(settings => settings.GetSettings()).Returns(_fixture.Create<MongoDBSettings>());

        // Act
        var clientProvider = new MongoClientProvider(mockSettings.Object);
        clientProvider.Client.Should().NotBeNull();
    }
}