using Microsoft.Azure.Cosmos;
using Nest;
using Products.Models;

namespace Products.Data
{
    public static class SynchroniseDataToElastic
    {
        public static async Task SynchroniseElasticData(this WebApplication app, IConfigurationRoot configuration)
        {

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var elasticsearchClient = services.GetRequiredService<ElasticClient>();
                    var cosmosClient = services.GetRequiredService<CosmosClient>();

                    var DatabaseName = configuration.GetSection("CosmosDBSettings:DatabaseName").Value;
                    var DefaultCollectionName = configuration.GetSection("CosmosDBSettings:DefaultCollectionName").Value;

                    var cosmosContainer = cosmosClient.GetContainer(DatabaseName, DefaultCollectionName);

                    var feedIterator = cosmosContainer.GetChangeFeedIterator<Product>(ChangeFeedStartFrom.Beginning(), ChangeFeedMode.Incremental);

                    while (feedIterator.HasMoreResults)
                    {
                        var feedResponse = await feedIterator.ReadNextAsync();

                        foreach (var changedDocument in feedResponse)
                        {
                            var product = changedDocument;
                            try
                            {
                                product.Description = product.Description.Replace("'", "");
                                product.Title = product.Title.Replace("'", "");
                            } catch  { }

                            var indexResponse = await elasticsearchClient.IndexDocumentAsync(product);
                            if (!indexResponse.IsValid)
                            {
                                Console.WriteLine($"Failed to index document {product.Id} in Elasticsearch: {indexResponse.OriginalException}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
