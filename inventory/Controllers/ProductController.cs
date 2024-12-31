using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inventory.Models;
using Microsoft.AspNetCore.Authorization;

namespace inventory.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    //[Authorize]
    
}
