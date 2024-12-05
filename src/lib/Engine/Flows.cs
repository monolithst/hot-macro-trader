using System.Collections.Immutable;
using Hmt.Lib.Market;

namespace Hmt.Lib.Engine;

public class Flows {
  private readonly IFlowsProps _flowsProps;

  public Flows(IFlowsProps props) {
    _flowsProps = props;
  }

  public void MaybeExecute(IHotKey hotkey) {
    MaybeExecute(new SimpleMacro(){
      Name = hotkey.Name,
      Description = hotkey.Description,
      Key = hotkey.Key,
      Actions = hotkey.Actions,
      Conditions = ImmutableArray<ICondition>.Empty,
      ExpiresInSeconds = null
    });
  }

  /**
   * This function will be executed many times (perhaps every tick) and
   * will determine if a macro should actually execute.
   */
  public async Task<bool> ShouldExecuteNow(IEngagedMacro engagedMacro) {
    // Has time expired?
    if (EngineLibs.IsExpired(engagedMacro, DateTime.Now)) {
      return false;
    }
    var macro = engagedMacro.Macro;
    
    // Do we have no conditions, as in, do it without any condition.
    if (macro.Conditions == null || macro.Conditions?.Length == 0) {
      return true;
    }

    var marketContext = await _flowsProps.Services.Market.GetMarketContext();
    var conditionFunc = EngineLibs.HasAnyConditionBeenMet(marketContext);
    return conditionFunc(macro.Conditions);
  }
}