using Grpc.Core;

namespace AuctionService.Services;

public class GrpcAuctionService(AuctionDbContext dbContext)
: GrpcAuction.GrpcAuctionBase
{

  public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
  {
    Console.WriteLine("gRPC GetAuction called");

    var auction = await dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
    ?? throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, "Auction not found"));

    var response = new GrpcAuctionResponse
    {
      Auction = new GrpcAuctionModel
      {
        AuctionEnd = auction.AuctionEnd.ToString(),
        Id = auction.Id.ToString(),
        ReservePrice = auction.ReservePrice,
        Seller = auction.Seller
      }
    };

    return response;
  }
}
