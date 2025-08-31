using AspNetCoreMvcPractice.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

public class ProductsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _apiBaseUrl;

    //IoC container 注入 IHttpClientFactory
    public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

        _apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/products");

        List<ProductDto> products = new();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            products = JsonSerializer.Deserialize<List<ProductDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return View(products);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var httpClient = _httpClientFactory.CreateClient();
        var product = await httpClient.GetFromJsonAsync<ProductDto>($"{_apiBaseUrl}/api/products/{id}");
        if (product == null) return NotFound();
        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductName,UnitPrice,UnitsInStock")] ProductDto product)
    {
        if (ModelState.IsValid)
        {
            var httpClient = _httpClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/products", product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var httpClient = _httpClientFactory.CreateClient();
        var product = await httpClient.GetFromJsonAsync<ProductDto>($"{_apiBaseUrl}/api/products/{id}");
        if (product == null) return NotFound();
        return View(product);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,UnitPrice,UnitsInStock")] ProductDto product)
    {
        if (id != product.ProductId) return NotFound();

        if (ModelState.IsValid)
        {
            var httpClient = _httpClientFactory.CreateClient();
            await httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/products/{id}", product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var httpClient = _httpClientFactory.CreateClient();
        var product = await httpClient.GetFromJsonAsync<ProductDto>($"{_apiBaseUrl}/api/products/{id}");
        if (product == null) return NotFound();
        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        await httpClient.DeleteAsync($"{_apiBaseUrl}/api/products/{id}");
        return RedirectToAction(nameof(Index));
    }
}