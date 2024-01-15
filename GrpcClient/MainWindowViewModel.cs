using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcProtoClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcClient
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            NormalGrpcClickCommand = new RelayCommand(NormalGrpcClickAction);
            ClientStreamClickCommand = new RelayCommand(ClientStreamClickAction);
            ServerStreamClickCommand = new RelayCommand(ServerStreamClickAction);
            BothStreamClickCommand = new RelayCommand(BothStreamClickAction);

            InitialGrpcClient();
        }
        public RelayCommand NormalGrpcClickCommand { get; set; }
        public RelayCommand ClientStreamClickCommand { get; set; }
        public RelayCommand ServerStreamClickCommand { get; set; }
        public RelayCommand BothStreamClickCommand { get; set; }

        private string normalData;

        public string NormalData
        {
            get => normalData;
            set => SetProperty(ref normalData, value);
        }

        private string clientStreamData;

        public string ClientStreamData
        {
            get => clientStreamData;
            set => SetProperty(ref clientStreamData, value);
        }

        private string serverStreamData;

        public string ServerStreamData
        {
            get => serverStreamData;
            set => SetProperty(ref serverStreamData, value);
        }

        private string bothStreamData;

        public string BothStreamData
        {
            get => bothStreamData;
            set => SetProperty(ref bothStreamData, value);
        }

        private Test.TestClient client;
        private void InitialGrpcClient()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:8899");
            client = new Test.TestClient(channel);
        }

        private async void NormalGrpcClickAction()
        {
            //NormalData=client.NormalSayHello(new HelloRequest { Name=""})
            var data = await client.NormalSayHelloAsync(new HelloRequest { Name = "This is Normal Request" });

            NormalData = data.Message;
        }

        private async void ClientStreamClickAction()
        {
            using var call = client.ClientStreamSayHello();
            for (int i = 1; i < 4; i++)
            {
                await call.RequestStream.WriteAsync(new HelloRequest() { Name = "This is ClientStream Request, Index=" + i.ToString() });
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
            await call.RequestStream.CompleteAsync();
            var response = await call;
            ClientStreamData = response.Message;
        }

        private async void ServerStreamClickAction()
        {
            using var call = client.ServerStreamSayHello(new HelloRequest() { Name="This is ServerStream Request"});
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                ServerStreamData += response.Message;
            }
        }

        private async void BothStreamClickAction()
        {
            using var call = client.BothStreamSayHello();
            for (int i = 1; i < 4; i++)
            {
                await call.RequestStream.WriteAsync(new HelloRequest() { Name = "This is BothStreamSayHello Request, Index=" + i.ToString() });
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await foreach(var response in call.ResponseStream.ReadAllAsync()) 
            { 
                BothStreamData += response.Message; 
            }
        }
    }
}
