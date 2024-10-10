using Microsoft.AspNetCore.Http;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
  private readonly Mock<IAuctionRepository> _repo;
  private readonly Mock<IPublishEndpoint> _publishEndpoint;
  private readonly IMapper _mapper;
  private readonly Fixture _fixture;
  private readonly AuctionsController _controller;

  public AuctionControllerTests()
  {
    _fixture = new Fixture();
    _repo = new Mock<IAuctionRepository>();
    _publishEndpoint = new Mock<IPublishEndpoint>();

    var mockMapper = new MapperConfiguration(cfg =>
    {
      cfg.AddMaps(typeof(MappingProfiles).Assembly);
    }).CreateMapper().ConfigurationProvider;

    _mapper = new Mapper(mockMapper);
    _controller = new AuctionsController(_repo.Object, _mapper, _publishEndpoint.Object)
    {
      ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext
        {
          User = Helpers.GetClaimsPrincipal()
        }
      }
    };
  }

  [Fact]
  public async Task GetAllAuctions_WithNoParams_ReturnsAllAuctions()
  {
    var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
    _repo.Setup(r => r.GetAuctionsAsync(null)).ReturnsAsync(auctions);

    var result = await _controller.GetAllAuctions(null);

    Assert.Equal(10, result.Value!.Count);
    Assert.IsType<ActionResult<List<AuctionDto>>>(result);
  }

  [Fact]
  public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
  {
    var auction = _fixture.Create<AuctionDto>();
    _repo.Setup(r => r.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

    var result = await _controller.GetAuctionById(Guid.NewGuid());

    Assert.Equal(auction.Make, result.Value!.Make);
    Assert.IsType<ActionResult<AuctionDto>>(result);
  }

  [Fact]
  public async Task GetAuctionById_WithInValidGuid_ReturnsNotFound()
  {
    _repo.Setup(r => r.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    var result = await _controller.GetAuctionById(Guid.NewGuid());

    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtAction()
  {
    var auction = _fixture.Create<CreateAuctionDto>();
    _repo.Setup(r => r.AddAuction(It.IsAny<Auction>()));
    _repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

    var result = await _controller.CreateAuction(auction);
    var createdResult = result.Result as CreatedAtActionResult;

    Assert.NotNull(createdResult);
    Assert.Equal("GetAuctionById", createdResult.ActionName);
    Assert.IsType<AuctionDto>(createdResult.Value);
  }

  [Fact]
  public async Task CreateAuction_FailedSave_Returns400BadRequest()
  {
    var auction = _fixture.Create<CreateAuctionDto>();
    _repo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

    var result = await _controller.CreateAuction(auction);

    Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsOkResponse()
  {
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Seller = "test";
    var updateDto = _fixture.Create<UpdateAuctionDto>();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    var result = await _controller.UpdateAuction(auction.Id, updateDto);

    Assert.IsType<OkObjectResult>(result.Result);
  }

  [Fact]
  public async Task UpdateAuction_WithInvalidUser_Returns403Forbid()
  {
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Seller = "not-test";
    var updateDto = _fixture.Create<UpdateAuctionDto>();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

    var result = await _controller.UpdateAuction(auction.Id, updateDto);

    Assert.IsType<ForbidResult>(result.Result);
  }

  [Fact]
  public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
  {
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    var updateDto = _fixture.Create<UpdateAuctionDto>();
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    var result = await _controller.UpdateAuction(auction.Id, updateDto);

    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task DeleteAuction_WithValidUser_ReturnsOkResponse()
  {
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Seller = "test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    var result = await _controller.DeleteAuction(auction.Id);

    Assert.IsType<NoContentResult>(result);
  }

  [Fact]
  public async Task DeleteAuction_WithInvalidGuid_Returns404Response()
  {
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Seller = "test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    var result = await _controller.DeleteAuction(auction.Id);

    Assert.IsType<NotFoundResult>(result);
  }

  [Fact]
  public async Task DeleteAuction_WithInvalidUser_Returns403Response()
  {
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Seller = "not-test";
    _repo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    var result = await _controller.DeleteAuction(auction.Id);

    Assert.IsType<ForbidResult>(result);
  }

}
