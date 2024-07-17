using Newtonsoft.Json;
using Ocelot.Configuration.File;
using System.Reflection;

namespace APIGateway.Extensions
{
    public static class OcelotBuilderExtensions
    {
        public static IConfigurationBuilder AddOcelotConfigFiles(this IConfigurationBuilder builder, string namespaceKey, string[] appNames)
        {
            const string primaryConfigFile = "ocelot.json";

            const string globalConfigFile = "ocelot.Base.json";

            List<FileInfo> files = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .EnumerateFiles()
                .Where(f => appNames.Any(e => f.Name.Contains(e)))
                .ToList();

            FileConfiguration fileConfiguration = new();

            foreach (FileInfo file in files)
            {
                if (file.Name.Equals(primaryConfigFile, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string lines = File.ReadAllText(file.FullName);

                FileConfiguration? config = JsonConvert.DeserializeObject<FileConfiguration>(lines);

                if (file.Name.Equals(globalConfigFile, StringComparison.OrdinalIgnoreCase))
                {
                    fileConfiguration.GlobalConfiguration = config.GlobalConfiguration;
                }

                fileConfiguration.Aggregates.AddRange(config.Aggregates);

                fileConfiguration.Routes.AddRange(config.Routes);
            }

            string json = JsonConvert.SerializeObject(fileConfiguration);

            //Replace {Namespace} by NAMESPACE for send request by environment
            json = json.Replace(".{Namespace}", "-svc." + namespaceKey);


            File.WriteAllText(primaryConfigFile, json);

            builder.AddJsonFile(primaryConfigFile, optional: false, reloadOnChange: true);

            return builder;
        }
    }
}
