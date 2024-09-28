using AuctionService;
using Grpc.Net.Client;

namespace BiddingService.Services;

public class GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
{

  public Auction? GetAuction(string id)
  {
    logger.LogInformation("Calling gRPC service");

    var address = config["GrpcAuction"] ?? throw new ArgumentNullException("GrpcAuction");

    var channel = GrpcChannel.ForAddress(address);
    var client = new GrpcAuction.GrpcAuctionClient(channel);
    var request = new GetAuctionRequest { Id = id };

    try
    {
      var reply = client.GetAuction(request);
      var auction = new Auction()
      {
        ID = reply.Auction.Id,
        AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
        Seller = reply.Auction.Seller,
        ReservePrice = reply.Auction.ReservePrice
      };

      return auction;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error calling gRPC Server");

      return null;
    }
  }

}
