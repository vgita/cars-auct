using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTests
{
    [Fact]
    public void HasReservePrice_ReservePriceGt0_True()
    {
        var entity = new Auction { Id = Guid.NewGuid(), ReservePrice = 10, Seller = "seller" };

        var result = entity.HasReservePrice();

        Assert.True(result);
    }

    [Fact]
    public void HasReservePrice_ReservePriceIs0_False()
    {
        var entity = new Auction { Id = Guid.NewGuid(), Seller = "seller" };

        var result = entity.HasReservePrice();

        Assert.False(result);
    }
}