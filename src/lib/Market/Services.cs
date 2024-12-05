namespace Hmt.Lib.Market;

public interface IMarketContext {
  IMarketData MarketData { get; }
}

public class MarketServices {
  public MarketServices() {
  }

  public Task<IMarketContext> GetMarketContext() {
    throw new NotImplementedException();
  }
}
  
