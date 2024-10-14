namespace AuctionService.IntegrationTests;

//public class AuctionBusTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
[Collection("SharedCollection")]
public class AuctionBusTests : IAsyncLifetime
{
  private readonly CustomWebAppFactory _factory;
  private readonly HttpClient _httpClient;
  private readonly ITestHarness _testHarness;

  public AuctionBusTests(CustomWebAppFactory factory)
  {
    _factory = factory;
    _httpClient = factory.CreateClient();
    _testHarness = factory.Services.GetTestHarness();
  }

  [Fact]
  public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
  {
    var auction = DataHelper.GetAuctionForCreate();
    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

    var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

    response.EnsureSuccessStatusCode();
    Assert.True(await _testHarness.Published.Any<AuctionCreated>());
  }

  public Task InitializeAsync() => Task.CompletedTask;

  public Task DisposeAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
    DbHelper.ReInitDbForTests(db);
    return Task.CompletedTask;
  }
}
