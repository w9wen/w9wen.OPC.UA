using Microsoft.AspNetCore.SignalR.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace w9wen.OPC.UA.Mobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private HubConnection hubConnection;
        private bool connected;
        private string readDateTime;
        private float vel;
        private float ve;
        private float vca;
        private float vc;

        public string ReadDateTime { get => readDateTime; set => SetProperty(ref readDateTime, value); }
        public float Vel { get => vel; set => SetProperty(ref vel, value); }
        public float Ve { get => ve; set => SetProperty(ref ve, value); }
        public float Vca { get => vca; set => SetProperty(ref vca, value); }
        public float Vc { get => vc; set => SetProperty(ref vc, value); }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "OPC UA Monitor";
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://w9wen-vidly.azurewebsites.net/OPCUAHub")
                .Build();

            hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await hubConnection.StartAsync();
            };

            await hubConnection.StartAsync();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    switch (user)
                    {
                        case "OPC_DateTime":
                            this.ReadDateTime = message;
                            break;

                        case "Vel":
                            this.Vel = float.Parse(message);
                            break;

                        case "Ve":
                            this.Ve = float.Parse(message);
                            break;

                        case "Vca":
                            this.Vca = float.Parse(message);
                            break;

                        case "Vc":
                            this.Vc = float.Parse(message);
                            break;

                        default:
                            break;
                    }
                });
            });
        }
    }
}