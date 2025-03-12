namespace inventory.Services.ProductRepo
{

    //method to check and restock product every 10 minutes
    public class ProductCheckService : BackgroundService
    {
       
        private readonly IServiceProvider _serviceProvider;

        public ProductCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {using (var scope = _serviceProvider.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    await productService.CheckAndRestockProduct();
                    await productService.CheckAndNotififyIfProductIsLessThanProedicted();
                }
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Check every 10 minutes
            }
        }
    }
}


