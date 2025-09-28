using Microsoft.Extensions.Configuration;
using System;
using System.IO;

public class ConfigurationSetting
{
    private readonly IConfigurationRoot _configuration;
    private readonly string _scenario;

    public ConfigurationSetting(string configFile = "appsettings.json")
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFile, optional: false, reloadOnChange: true);

        _configuration = builder.Build();

        _scenario = _configuration["Scenario"];
        if (string.IsNullOrWhiteSpace(_scenario))
            throw new Exception("Scenario is not defined in appsettings.json");
    }

    public string GetExpectedPath()
    {
        var pathTemplate = _configuration["Paths:ExpectedPath"];
        return pathTemplate.Replace("{scenario}", _scenario, StringComparison.OrdinalIgnoreCase);
    }

    public string GetActualPath()
    {
        var pathTemplate = _configuration["Paths:ActualPath"];
        return pathTemplate.Replace("{scenario}", _scenario, StringComparison.OrdinalIgnoreCase);
    }
}
