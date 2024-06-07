﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ITI.Grpc.Protos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static GRPC.Protos.InventoryServiceProto;

namespace GRPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : Controller
    {
        private readonly IConfiguration _configuration;

        public StoreController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult> AllProducts()
        {
            var apiKey = _configuration["ApiKey"];

            var channel = GrpcChannel.ForAddress("https://localhost:7214", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", apiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryServiceProto.InventoryServiceProtoClient(channel);

            try
            {
                var products = await client.GetAllAsync(new Google.Protobuf.WellKnownTypes.Empty(), new CallOptions(credentials: callCredentials));
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving all products: " + ex.Message);
                return StatusCode(500, "Internal server error occurred while retrieving all products: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            var apiKey = _configuration["ApiKey"];

            var channel = GrpcChannel.ForAddress("https://localhost:7214", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", apiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryServiceProto.InventoryServiceProtoClient(channel);

            try
            {
                var isExisted = await client.GetProductByIdAsync(new Id { Id_ = product.Id }, new CallOptions(credentials: callCredentials));

                if (!isExisted.IsExisted_)
                {
                    var addedProduct = await client.AddProductAsync(product, new CallOptions(credentials: callCredentials));
                    return Created("Product Created", addedProduct);
                }

                var updatedProduct = await client.UpdateProductAsync(product, new CallOptions(credentials: callCredentials));
                return Created("Product Updated", updatedProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding or updating product: " + ex.Message);
                return StatusCode(500, "Internal server error occurred while adding or updating the product: " + ex.Message);
            }
        }

        [HttpPost("addproducts")]
        public async Task<ActionResult> AddBulkProducts(List<Product> productToAdds)
        {
            var apiKey = _configuration["ApiKey"];

            var channel = GrpcChannel.ForAddress("https://localhost:7214", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", apiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryServiceProto.InventoryServiceProtoClient(channel);

            try
            {
                var call = client.AddBulkProducts(new CallOptions(credentials: callCredentials));

                foreach (var product in productToAdds)
                {
                    await call.RequestStream.WriteAsync(product);
                    await Task.Delay(100); 
                }

                await call.RequestStream.CompleteAsync();
                var response = await call.ResponseAsync;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding bulk products: " + ex.Message);
                return StatusCode(500, "Internal server error occurred while adding bulk products: " + ex.Message);
            }
        }

        [HttpGet("GetReport")]
        public async Task<ActionResult> GetReport()
        {
            var apiKey = _configuration["ApiKey"];

            var channel = GrpcChannel.ForAddress("https://localhost:7214", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", apiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryServiceProto.InventoryServiceProtoClient(channel);

            List<Product> addedProducts = new List<Product>();

            try
            {
                var call = client.GetProductReport(new Google.Protobuf.WellKnownTypes.Empty(), new CallOptions(credentials: callCredentials));

                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    addedProducts.Add(call.ResponseStream.Current);
                }

                return Ok(addedProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving product report: " + ex.Message);
                return StatusCode(500, "Internal server error occurred while retrieving product report: " + ex.Message);
            }
        }
    }
}
