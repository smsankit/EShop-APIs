//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using Nest;
//using Products.Data;
//using Products.Models;

//[Route("[controller]")]
//[EnableCors]
//[ApiController]
//[ExcludeFromDescription]
//[ExcludeFromCodeCoverage]
//public class ProductsOldController : ControllerBase
//{
//    private readonly ProductsDbContext _context;
//    private readonly ElasticClient _client;

//    public ProductsOldController(ProductsDbContext context, ElasticClient? client)
//    {
//        _context = context;
//        _client = client;
//    }


//    [HttpGet("search")]
//    public async Task<IActionResult> SearchAsync(string searchTerm)
//    {
//        searchTerm = searchTerm.Trim();
//        try
//        {
//            var searchResponse = await _client.SearchAsync<Product>(s => s
//            .Query(q => q
//                .Bool(b => b
//                    .Should(sh => sh
//                        .Wildcard(w => w
//                            .Field(f => f.Title)
//                            .Value($"*{searchTerm.ToLowerInvariant()}*")
//                        ),
//                        sh => sh
//                        .Wildcard(w => w
//                            .Field(f => f.Category)
//                            .Value($"*{searchTerm.ToLowerInvariant()}*")
//                        ),
//                        sh => sh
//                        .Wildcard(w => w
//                            .Field(f => f.Description)
//                            .Value($"*{searchTerm.ToLowerInvariant()}*")
//                        ),
//                        sh => sh
//                        .Wildcard(w => w
//                            .Field(f => f.Price)
//                            .Value($"*{searchTerm.ToLowerInvariant()}*")
//                        ),
//                        sh => sh
//                        .Wildcard(w => w
//                            .Field(f => f.Id)
//                            .Value($"*{searchTerm.ToLowerInvariant()}*")
//                        ),
//                        sh => sh
//                        .MatchPhrase(m => m
//                        .Field(f => f.Title) // Replace "YourField" with the field you want to search in
//                        .Query(searchTerm.ToLowerInvariant())
//                        ),
//                        sh => sh
//                        .MatchPhrase(m => m
//                        .Field(f => f.Description) // Replace "YourField" with the field you want to search in
//                        .Query(searchTerm.ToLowerInvariant())
//                        ),
//                        sh => sh
//                        .MatchPhrase(m => m
//                        .Field(f => f.Category) // Replace "YourField" with the field you want to search in
//                        .Query(searchTerm.ToLowerInvariant())
//                        )
//                    )
//                )
//            )
//        );


//            // Check if the search was successful
//            if (searchResponse.IsValid)
//            {
//                // Process and return the search results
//                return Ok(searchResponse.Documents);
//            }
//            else
//            {
//                // Handle search failure
//                return StatusCode(500, $"Search failed. Error: {searchResponse.ServerError}");
//            }
//        }
//        catch (Exception ex)
//        {
//            // Handle other exceptions
//            return StatusCode(500, $"Error: {ex.Message}");
//        }
//    }

//    [HttpGet("search/category")]
//    public async Task<IActionResult> SearchByCategoryAsync(string category)
//    {
//        //category = category.Trim();
//        try
//        {
//            var searchResponse = await _client.SearchAsync<Product>(s => s
//            .Query(q => q
//                .Bool(b => b
//                    .Should(

//                        // Partial match using Match query
//                        m => m.Match(m => m.Field(f => f.Category).Query(category)),
//                        // Exact match using Term query
//                        t => t.Term(t => t.Field(f => f.Category).Value(category))
//                    )
//                )
//            )
//        );


//            if (searchResponse.IsValid)
//            {
//                return Ok(searchResponse.Documents);
//            }
//            else
//            {
//                // Handle search failure
//                return StatusCode(500, $"Search failed. Error: {searchResponse.ServerError}");
//            }
//        }
//        catch (Exception ex)
//        {
//            // Handle other exceptions
//            return StatusCode(500, $"Error: {ex.Message}");
//        }
//    }

//    [HttpOptions]
//    public IActionResult Options()
//    {
//        return Ok();
//    }


//    // GET: api/Products
//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
//    {
//        var products = await _context.Products.ToListAsync();

//        if (products == null)
//        {
//            return NotFound();
//        }

//        return products;
//    }

//    // GET: api/Products/5
//    [HttpGet("{id}")]
//    public async Task<ActionResult<Product>> GetProduct(string id)
//    {
//        if (_context.Products == null)
//        {
//            return NotFound();
//        }
//        var product = await _context.Products.FindAsync(id);

//        if (product == null)
//        {
//            return NotFound();
//        }

//        return product;
//    }

//    // PUT: api/Products/5
//    [HttpPut("{id}")]
//    public async Task<IActionResult> PutProduct(string id, Product product)
//    {
//        if (id != product.Id)
//        {
//            return BadRequest();
//        }

//        _context.Entry(product).State = EntityState.Modified;

//        try
//        {
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!ProductExists(id))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }

//        return NoContent();
//    }

//    // POST: api/Products
//    [HttpPost]
//    public async Task<ActionResult<Product>> PostProduct(Product product)
//    {
//        if (_context.Products == null)
//        {
//            return Problem("Entity set 'ProductsDbContext.Products'  is null.");
//        }
//        _context.Products.Add(product);
//        try
//        {
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateException)
//        {
//            if (ProductExists(product.Id))
//            {
//                return Conflict();
//            }
//            else
//            {
//                throw;
//            }
//        }

//        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
//    }


//    // POST: api/Products
//    [HttpPost("PostProductList")]
//    public async Task<ActionResult<Product>> PostProductList(List<Product> productList)
//    {
//        if (_context.Products == null)
//        {
//            return Problem("Entity set 'ProductsDbContext.Products'  is null.");
//        }
//        _context.Products.AddRange(productList);
//        try
//        {
//            await _context.SaveChangesAsync();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.ToString());
//        }

//        return Ok();
//    }

//    // DELETE: api/Products/5
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteProduct(string id)
//    {
//        if (_context.Products == null)
//        {
//            return NotFound();
//        }
//        var product = await _context.Products.FindAsync(id);
//        if (product == null)
//        {
//            return NotFound();
//        }

//        _context.Products.Remove(product);
//        await _context.SaveChangesAsync();

//        return NoContent();
//    }

//    private bool ProductExists(string id)
//    {
//        return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
//    }
//}