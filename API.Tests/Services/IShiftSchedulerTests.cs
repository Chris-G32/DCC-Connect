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
using MongoDB.Bson;
using MongoDB.Driver.Core.Misc;
using API.Errors;
using API.Constants.Errors;


namespace API.Tests.Services;
/// <summary>
/// These tests are fairly minimal to avoid testing the internals too much of the mongoClient class itself.
/// </summary>
[TestClass]
public class IShiftSchedulerTests
{
    private IFixture _fixture;
    private Mock<ILogger<ShiftScheduler>> _loggerMock;
    private Mock<IMongoCollection<Shift>> _shiftMock;
    private Mock<ICollectionsProvider> _mockCollectionsProvider;
    private Mock<IEntityRetriever> _entityRetriever;
    private Mock<IAvailabiltyService> _mockAvailabiltyService;

    void SetupShiftMock(Mock<IMongoCollection<Shift>> shiftMock, UpdateResult returnValue)
    {
        shiftMock.Setup(
            Shift =>
            Shift.UpdateOne(
                It.IsAny<FilterDefinition<Shift>>(),
                It.IsAny<UpdateDefinition<Shift>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()
                )).Returns(returnValue);
    }
    [TestInitialize]
    public void Setup()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        // Tell fixture how to auto generate certain types
        _fixture.Register(() =>
        {
            var start = DateTime.Now.AddDays(1);
            var end = start.AddHours(8);
            return _fixture.Build<TimeRange>().With(range => range.Start, start).With(range => range.End, end).Create();
        }
        );
        _fixture.Register(() =>
        {
            var randomBytes = _fixture.CreateMany<byte>(12).ToArray(); // Convert the byte array to a hexadecimal string
            var hexString = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
            return ObjectId.Parse(hexString);
        });

        _loggerMock = _fixture.Freeze<Mock<ILogger<ShiftScheduler>>>();
        _shiftMock = _fixture.Freeze<Mock<IMongoCollection<Shift>>>();
        _mockCollectionsProvider = _fixture.Freeze<Mock<ICollectionsProvider>>();
        _entityRetriever = _fixture.Freeze<Mock<IEntityRetriever>>();
        _mockAvailabiltyService = _fixture.Freeze<Mock<IAvailabiltyService>>();
    }

    [TestMethod]
    [DataRow(null, false, DisplayName = "Case 1: Should throw error saying employee unschedulable")]
    [DataRow(false, true, DisplayName = "Case 2: Should throw an exception due to failed assignment")]
    [DataRow(true, true, DisplayName = "Case 3: Should not throw any exception")]
    public void TestAssignShift(bool? updateSucceeds, bool isEmployeeSchedulable)
    {
        // Arrange
        if (updateSucceeds != null)
        {
            var updateResult = (bool)updateSucceeds ? new UpdateResult.Acknowledged(1, 1, 1) : new UpdateResult.Acknowledged(0, 0, 0);
            SetupShiftMock(_shiftMock, updateResult);
        }
        _mockAvailabiltyService
            .Setup(serv => serv.IsEmployeeSchedulableForShift(It.IsAny<ObjectId>(), It.IsAny<ObjectId>()))
            .Returns(isEmployeeSchedulable);

        var scheduler = _fixture.Create<ShiftScheduler>();
        var shiftAssignment = _fixture.Create<ShiftAssignment>();

        // Act & Assert
        var act = () => scheduler.AssignShift(shiftAssignment);
        int expectedTimes = 1;
        if (isEmployeeSchedulable==false)
        {
            expectedTimes = 0;
            Console.WriteLine("Ran should throw employee unschedulable sut.");
            act.Should().Throw<DCCApiException>().WithMessage(ShiftSchedulerErrorConstants.EmployeeNotSchedulableError);
        }
        else if (updateSucceeds==false)
        {
            Console.WriteLine("Ran should throw update fail sut.");
            act.Should().Throw<DCCApiException>().WithMessage(ShiftSchedulerErrorConstants.ShiftAssignmentUpdateFailedError);
        }
        else
        {
            Console.WriteLine("Ran should not throw sut.");
            act.Should().NotThrow<Exception>();
        }

        _shiftMock.Verify(collection => collection.UpdateOne(
                It.IsAny<FilterDefinition<Shift>>(),
                It.IsAny<UpdateDefinition<Shift>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(expectedTimes),
            "Update was called an unexpected number of times.");
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

    [TestMethod]
    public void TestCreateShift()
    {
        var random = new Random();

        var getRandomDouble = (double minValue, double maxValue) => random.NextDouble() * (maxValue - minValue);
        var tryCreatePastShift = () =>
        {
            // Construct a shift that starts in the past
            var startTime = DateTime.Now.AddDays(getRandomDouble(1, 100) * -1);
            var range = _fixture.Build<TimeRange>()
            .With(tr => tr.Start, startTime).With(tr => tr.End, startTime.AddHours(7))
            .Create();
            var shift = _fixture.Build<Shift>()
            .With(shift => shift.ShiftPeriod, range)
            .Create();

            var scheduler = _fixture.Create<ShiftScheduler>();
            scheduler.CreateShift(shift);
        };
        var tryCreateFutureShift = () =>
        {
            //Construct a shift that starts tomorrow and ends 7 hours later
            var startTime = DateTime.Now.AddDays(getRandomDouble(1, 100));
            var range = _fixture.Build<TimeRange>()
            .With(tr => tr.Start, startTime).With(tr => tr.End, startTime.AddHours(7))
            .Create();
            var shift = _fixture.Build<Shift>()
            .With(shift => shift.ShiftPeriod, range)
            .Create();

            var scheduler = _fixture.Create<ShiftScheduler>();
            scheduler.CreateShift(shift);
        };

        tryCreatePastShift.Should().Throw<Exception>();
        _shiftMock.Verify(collection =>
            collection.InsertOne(It.IsAny<Shift>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
            Times.Never(), "Shift was not created.");

        tryCreateFutureShift.Should().NotThrow<Exception>();
        _shiftMock.Verify(collection =>
            collection.InsertOne(It.IsAny<Shift>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
            Times.Once(), "Shift was not created.");
    }
    [TestMethod]
    public void TestDeleteShift()
    {

        var tryDeleteShiftWithEmployee = () =>
        {
            var shift = _fixture.Build<Shift>().With(shift => shift.Id, _fixture.Create<ObjectId>()).With(shift => shift.EmployeeID, _fixture.Create<ObjectId>()).Create();
            _entityRetriever.Setup(et => et.GetEntityOrThrow(It.IsAny<IMongoCollection<Shift>>(), It.IsAny<ObjectId>()))
            .Returns(shift);
            var scheduler = _fixture.Create<ShiftScheduler>();
            var shiftId = shift.Id ?? throw new Exception("Id should not be null.");
            scheduler.DeleteShift(shiftId);
        };
        var tryDeleteShiftWithoutEmployee = () =>
        {
            var shift = _fixture.Build<Shift>().With(shift => shift.Id, _fixture.Create<ObjectId>()).Without(shift => shift.EmployeeID).Create();
            _entityRetriever.Setup(et => et.GetEntityOrThrow(It.IsAny<IMongoCollection<Shift>>(), It.IsAny<ObjectId>()))
            .Returns(shift);
            var scheduler = _fixture.Create<ShiftScheduler>();
            var shiftId = shift.Id ?? throw new Exception("Id should not be null.");
            scheduler.DeleteShift(shiftId);
        };

        tryDeleteShiftWithEmployee.Should().Throw<Exception>();
        _shiftMock.Verify(collection =>
            collection.FindOneAndDelete(It.IsAny<FilterDefinition<Shift>>(), It.IsAny<FindOneAndDeleteOptions<Shift>>(), It.IsAny<CancellationToken>()),
            Times.Never(), "Shift was deleted.");

        tryDeleteShiftWithoutEmployee.Should().NotThrow<Exception>();
        _shiftMock.Verify(collection =>
            collection.FindOneAndDelete(It.IsAny<FilterDefinition<Shift>>(), It.IsAny<FindOneAndDeleteOptions<Shift>>(), It.IsAny<CancellationToken>()),
            Times.Once(), "Shift was not deleted.");
    }

    [TestMethod]
    [DataRow(false, false, 0, 0)] // UnassignShiftWithNoAssignmentTest
    [DataRow(false, true, 1, 0)] // UnassignAssignedShiftTest
    [DataRow(true, false, 0, 0)]  // UnassignAssignedShiftWithCoverageRequestTest
    [DataRow(true, true, 1, 1)]  // UnassignedShiftWithLingeringCoverageRequestTest
    public void UnassignShiftTest(bool hasCoverageRequest, bool isShiftAssigned, int coverageRequestDeleteTimes, int tradeOfferDeleteTimes)
    {

        // Arrange
        Shift shift = isShiftAssigned ? _fixture.Create<Shift>() : _fixture.Build<Shift>().Without(s => s.EmployeeID).Create();
        CoverageRequest? coverage = hasCoverageRequest ? _fixture.Create<CoverageRequest>() : null;

        // Mocking the entity retrieval
        _entityRetriever.Setup(er => er.GetEntityOrThrow(It.IsAny<IMongoCollection<Shift>>(), It.IsAny<ObjectId>())).Returns(shift);

        // Mocking CoverageRequest behavior based on the test case
        _mockCollectionsProvider.Setup(collectionProvider => collectionProvider.CoverageRequests
            .FindOneAndDelete(It.IsAny<FilterDefinition<CoverageRequest>>(), It.IsAny<FindOneAndDeleteOptions<CoverageRequest>>(), It.IsAny<CancellationToken>())
        ).Returns(coverage);

        // Act
        var scheduler = _fixture.Create<ShiftScheduler>();
        scheduler.UnassignShift(shift.Id ?? throw new ArgumentNullException("Shift ID passed from unit test was null."));

        // Assert
        _mockCollectionsProvider.Verify(collection => collection.CoverageRequests.FindOneAndDelete(
                It.IsAny<FilterDefinition<CoverageRequest>>(),
                It.IsAny<FindOneAndDeleteOptions<CoverageRequest>>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(coverageRequestDeleteTimes), "FindOneAndDelete was not called the expected number of times.");

        _mockCollectionsProvider.Verify(provider => provider.TradeOffers.DeleteMany(
                It.IsAny<FilterDefinition<TradeOffer>>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(tradeOfferDeleteTimes), "TradeOffers.DeleteMany was not called the expected number of times.");
    }

}