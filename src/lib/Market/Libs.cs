namespace Hmt.Lib.Market;

public interface IMarketData {
  decimal Bid { get; }
  decimal Ask { get; }
  decimal Last { get; }
  decimal Volume { get; }
  decimal Open { get; }
  decimal Close { get; }
  decimal High { get; }
  decimal Low { get; }
  DateTime Timestamp { get; }
} 


public class Libs {
  
}