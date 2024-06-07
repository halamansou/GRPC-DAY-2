using GRPC.Protos;
using static GRPC.Protos.InventoryServiceProto;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;

namespace GRPC.Server.Services
{
    public class InventroyService : InventoryServiceProtoBase
    {
        public static List<Product> Products { get; set; }


        public InventroyService()
        {
            Products = new List<Product>()
        {
            new Product{Id=1,Name="Smartphone",Price=800,Quantity=10, Category=ProductCategory.Electronics, ExpiryDate=Timestamp.FromDateTime(DateTime.UtcNow.AddYears(1))},
            new Product{Id=2,Name="Keyboard",Price=100,Quantity=20, Category=ProductCategory.Electronics, ExpiryDate=Timestamp.FromDateTime(DateTime.UtcNow.AddYears(1))},
            new Product{Id=3,Name="Speakers",Price=150,Quantity=8, Category=ProductCategory.Electronics, ExpiryDate=Timestamp.FromDateTime(DateTime.UtcNow.AddYears(1))},
            new Product{Id=4,Name="Digital Camera",Price=1200,Quantity=3, Category=ProductCategory.Electronics, ExpiryDate=Timestamp.FromDateTime(DateTime.UtcNow.AddYears(1))},
            new Product{Id=5,Name="Sweater",Price=50,Quantity=15, Category=ProductCategory.Clothes, ExpiryDate=Timestamp.FromDateTime(DateTime.UtcNow.AddYears(1))},
            new Product{Id=6,Name="Jeans",Price=70,Quantity=10, Category=ProductCategory.Clothes, ExpiryDate=Timestamp.FromDateTime(DateTime.UtcNow.AddYears(1))}
            };

        }
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]

       
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<IsExisted> GetProductById(Id request, ServerCallContext context)
        {
            var product = Products.FirstOrDefault(i => i.Id == request.Id_);
            if (product != null)
            {
                return await Task.FromResult(new IsExisted
                {
                    IsExisted_ = true
                }); ;
            }
            return await Task.FromResult(new IsExisted
            {
                IsExisted_ = false
            }); ;
            //return base.GetProductById(request, context);
        }


        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]

        public override async Task<Product> AddProduct(Product request, ServerCallContext context)
        {
            Products.Add(request);
            return await Task.FromResult(request);

        }


        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]

        public override async Task<Product> UpdateProduct(Product request, ServerCallContext context)

        {
            var product = Products.FirstOrDefault(i => i.Id == request.Id);
            if (product != null)
            {
                product.Name = request.Name;
                product.Price = request.Price;
                product.Quantity = request.Quantity;
            }


            return await Task.FromResult(product);
        }
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]

        public override async Task<productsNumber> AddBulkProducts(IAsyncStreamReader<Product> requestStream, ServerCallContext context)
        {
            int count = 0;
            await foreach (var request in requestStream.ReadAllAsync())
            {
                Products.Add(request);
                ++count;
            }

            return await Task.FromResult(new productsNumber { Count = count });

        }
        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]

        public override async Task GetProductReport(Empty request, IServerStreamWriter<Product> responseStream, ServerCallContext context)
        {
            foreach (var item in Products)
            {
                await responseStream.WriteAsync(item);
            }

            await Task.CompletedTask;
        }
    }
}
