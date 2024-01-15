using Grpc.Core;
using GrpcProtoClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServer
{
    public class TestBiz : Test.TestBase
    {
        public override Task<HelloResponse> NormalSayHello(HelloRequest request, ServerCallContext context)
        {
            var response = new HelloResponse()
            {
                Message = "This is normal response from normal request",
            };
            return Task.FromResult(response);
        }

        public override async Task<HelloResponse> ClientStreamSayHello(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var readData = "";
            await foreach (var request in requestStream.ReadAllAsync())
            {
                readData += $"{request.Name} \r\n";
            }
            return new HelloResponse()
            {
                Message = readData,
            };
        }

        public override async Task ServerStreamSayHello(HelloRequest request, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {
            for (int i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new HelloResponse()
                {
                    Message = $"This is normal request and Server Stream response {DateTime.Now.ToLongTimeString()} \r\n",
                });
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        public override async Task BothStreamSayHello(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {

            var readData = "";

            // Read requests in a background task.
            var readTask = Task.Run(async () =>
            {
                await foreach (var request in requestStream.ReadAllAsync())
                {
                    readData += $"{request.Name}  {DateTime.Now.ToLongTimeString()} \r\n";
                }
            });

            // Send responses until the client signals that it is complete.
            while (!readTask.IsCompleted)
            {
                await responseStream.WriteAsync(new HelloResponse()
                {
                    Message= readData,
                });
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }
    }
}
