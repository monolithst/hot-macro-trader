using System.Collections.Immutable;
using Hmt.Lib.Market;

namespace Hmt.Lib.Engine;

public interface IServices {
  EngineServices Engine { get; }
  MarketServices Market { get; }
}

public interface IFlowsProps {
  IServices Services { get; }
}

public class MarketContext : IMarketContext {
  public required IMarketData MarketData { get; init; }
}

public interface IHotKey {
  string Name { get; }
  string Description { get; }
  string Key { get; }
  ImmutableArray<IAction> Actions { get; }
}

public enum ConditionTarget {
  Bid,
  Ask,
  Last,
  Volume,
  Open,
  Close,
  High,
  Low,
  Timestamp
}

public enum Equality {
  Equal,
  GreaterThan,
  LessThan,
  GreaterThanOrEqual,
  LessThanOrEqual,
}

public interface ICondition {
  ConditionTarget Target { get; }
  Equality Equality { get; }
  decimal Value { get; }
  ImmutableArray<ICondition>? SubConditions { get; }
}

public interface IGoodFor: ICondition {
  DateTime ExecuteBy { get; }
}

public enum BuyOrSell {
  Buy,
  Sell,
}

public enum OrderType {
  Market,
  Limit,
  Stop,
}

public interface IAction {
  BuyOrSell BuyOrSell { get; }
  OrderType OrderType { get; }
}

public interface IMacro : IHotKey {
  /*
   * Each condition is an OR.
   * AND conditions are created via SubConditions. (Which multiple are ORs as well).
   *
   * Examples:
   * 1: A AND B
   * {
   *   Conditions: [
   *    (A) {
   *       SubConditions: [
   *          B,
   *       ]
   *    }
   *   ]
   * }
   *
   * 2: A OR B
   * {
   *   Conditions: [
   *    A,
   *    B,
   *   ]
   * }
   * 
   * 3: A AND B AND C
   * {
   *   Conditions: [
   *    (A) {
   *       SubConditions: [
   *          (B) {
   *            SubConditions: [
   *              C 
   *            ]
   *          },
   *       ]
   *    }
   *   ]
   * }
   *
   * 4: A AND (B OR C)
   * {
   *   Conditions: [
   *    (A) {
   *       SubConditions: [
   *          B,
   *          C,
   *       ]
   *    }
   *   ]
   * }
   * (A AND B) OR C
   * {
   *   Conditions: [
   *    (A) {
   *       SubConditions: [
   *          B,
   *       ]
   *    },
   *    C
   *   ]
   * }
   */
  ImmutableArray<ICondition>? Conditions { get; }
  int? ExpiresInSeconds { get; }
}

public interface IEngagedMacro {
  IMacro Macro { get; }
  DateTime ExecutedOn { get; }
}

public record SimpleMacro : IMacro {
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string Key { get; init; } = string.Empty;
  public ImmutableArray<IAction> Actions { get; init; } = ImmutableArray<IAction>.Empty;
  public ImmutableArray<ICondition>? Conditions { get; init; } = ImmutableArray<ICondition>.Empty;
  /**
   * When a macro will no longer take affect.
   *
   * Example: Useful for automating a future buy order
   * that expires because a price hasn't been reached yet.
   */
  public int? ExpiresInSeconds { get; init; }
} 


public static class EngineLibs {
  public static bool IsExpired(IEngagedMacro engagedMacro, DateTime now) {
    if (engagedMacro.Macro.ExpiresInSeconds == null) {
      return false;
    }

    var secondsBetween = (now - engagedMacro.ExecutedOn).TotalSeconds;
    return engagedMacro.Macro.ExpiresInSeconds <= secondsBetween;
  }
  
  public static Func<ImmutableArray<ICondition>?, bool> HasAnyConditionBeenMet(IMarketContext marketContext) {
    return maybeConditions => {
      if (maybeConditions == null) {
        return true;
      }
      var checker = HasConditionBeenMet(marketContext);
      return (bool)maybeConditions?.Any(checker);
    };
  }

  public static Func<ICondition, bool> HasConditionBeenMet(IMarketContext marketContext) {
    return (c => {
      var subConditionsMet = HasAnyConditionBeenMet(marketContext)(c.SubConditions);
      if (subConditionsMet == false) {
        return false;
      }
      /*
       * TODO: Actually implement conditions
       */
      throw new NotImplementedException("Not implemented");
      return true;
    });
  }
}