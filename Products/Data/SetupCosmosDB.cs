using Products.Models;

namespace Products.Data
{
    public static class SetupCosmosDB
    {
        public static async Task<bool> CreateDBIfNotAlready(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<ProductsDbContext>();
                    if (dbContext.Database.EnsureCreated())
                    {
                        await SeedData(app);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        static async Task<bool> SeedData(WebApplication app)
        {
            string apiUrl = "https://fakestoreapi.com/products";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        List<Product> products = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Product>>(apiResponse);

                        if (products != null)
                        {
                            using (var scope = app.Services.CreateScope())
                            {
                                try
                                {
                                    var services = scope.ServiceProvider;
                                    var dbContext = services.GetRequiredService<ProductsDbContext>();
                                    dbContext.Products.AddRange(products);
                                    await dbContext.SaveChangesAsync();
                                    Console.WriteLine("Products inserted into the database successfully.");
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    return false;
                                }
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return false;
        }
    }
}
