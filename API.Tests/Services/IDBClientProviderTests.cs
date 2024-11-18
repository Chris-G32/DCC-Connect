using API.Services;

using Microsoft.Extensions.Configuration;
using API.Config;
using API.Constants;
using MongoDB.Driver;

namespace API.Tests.Services;
/// <summary>
/// These tests are fairly minimal to avoid testing the internals too much of the mongoClient class itself.
/// </summary>
[TestClass]
public class IDBClientProviderTests
{
    private Fixture _fixture;
    [TestInitialize]
    public void Setup()
    {
        _fixture = new Fixture();

        //_config = _fixture.Freeze<Mock<IConfiguration>>();
        //_config.Setup(conf => conf.GetRequiredSection(It.IsAny<string>()).Get<MongoDBSettings>()).Returns(new MongoDBSettings());

    }
    [TestMethod]
    public void NoNullConfig()
    {
        var sut = () => { var clientProvider = new MongoClientProvider(null); };
        sut.Should().Throw<Exception>();
    }
    [TestMethod]
    public void ClientIsNotNull()
    {
        // Arrange
        var mockSettings = new Mock<IMongoDBSettingsProvider>();

        mockSettings.Setup(settings=>settings.GetSettings()).Returns(_fixture.Create<MongoDBSettings>());

        // Act
        var clientProvider = new MongoClientProvider(mockSettings.Object);
        clientProvider.Client.Should().NotBeNull();
    }
}