namespace Hmt.lib.Configuration;

public interface IConfigurationServicesProps {
}

public enum Broker {
  Schwab
}

public interface IConfigurations {
  Broker broker { get; }
}

public class ConfigurationServices {
  private readonly IConfigurationServicesProps _props;
  public ConfigurationServices(IConfigurationServicesProps props) {
    _props = props; 
  }
  
  public void getConfigurations() {
    
  }
}