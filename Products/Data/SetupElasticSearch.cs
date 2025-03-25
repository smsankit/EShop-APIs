using Nest;
using Products.Models;

namespace Products.Data
{
    public static class SetupElasticSearch
    {
        public static void SetupElastic(this WebApplicationBuilder builder, IConfigurationRoot configuration)
        {
            try
            {
                var ElasticURL = configuration.GetSection("ElasticSearchSettings:ElasticURL").Value;
                var CertificateFingerprint = configuration.GetSection("ElasticSearchSettings:CertificateFingerprint").Value;
                var DefualtIndex = configuration.GetSection("ElasticSearchSettings:DefualtIndex").Value;
                var UserName = configuration.GetSection("ElasticSearchSettings:UserName").Value;
                var Password = configuration.GetSection("ElasticSearchSettings:Password").Value;

                var elasticsearchSettings = new ConnectionSettings(new Uri(ElasticURL))
                    .DefaultIndex(DefualtIndex)
                    //.CertificateFingerprint(CertificateFingerprint)
                    .BasicAuthentication(UserName, Password)
                    .PrettyJson();

                AddMappings(elasticsearchSettings);

                var elasticsearchClient = new ElasticClient(elasticsearchSettings);

                builder.Services.AddSingleton<ElasticClient>(elasticsearchClient);

                CreateIndex(elasticsearchClient, DefualtIndex);
            }
            catch (Exception ex)
            {
                builder.Services.AddSingleton<ElasticClient>(new ElasticClient());
                Console.WriteLine("Error setting up Elastic Client. Setting up empty object to run the app.");
            }
        }
        static void AddMappings(ConnectionSettings connectionSettings)
        {
            connectionSettings.DefaultMappingFor<Product>(p => p.Ignore(a => a.Rating));
        }
        static void CreateIndex(IElasticClient client, string indexName)
        {
            var indexExistsRequest = client.Indices.Exists(indexName);

            if (!indexExistsRequest.Exists)
            {
                var res = client.Indices.Create(indexName, i => i.Map<Product>(x => x.AutoMap()));
            }
        }
    }
}
