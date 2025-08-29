using AspNetCoreMvcPractice.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class ProductsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync("https://localhost:44399/api/products");

        List<ProductDto> products = new();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            products = JsonSerializer.Deserialize<List<ProductDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        return View(products);
    }
}