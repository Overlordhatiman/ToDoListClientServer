using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo;
using ToDoListClientApp.Models;

namespace ToDoListClientApp.Services
{
    public class ToDoServiceClient
    {
        private readonly ToDoService.ToDoServiceClient _client;

        public ToDoServiceClient(string address = "http://localhost:50051")
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new ToDoService.ToDoServiceClient(channel);
        }

        public async Task<ToDoItem> AddItemAsync(string description)
        {
            var request = new AddItemRequest { Description = description };
            var response = await _client.AddItemAsync(request);
            return response.Item;
        }

        public async Task<ToDoItem> ToggleStatusAsync(int id)
        {
            var request = new UpdateStatusRequest { Id = id };
            var response = await _client.UpdateStatusAsync(request);
            return response.Item;
        }

        public async Task<List<ToDoItem>> GetListAsync()
        {
            var items = new List<ToDoItem>();

            using var call = _client.GetList(new Empty());
            while (await call.ResponseStream.MoveNext())
            {
                items.Add(call.ResponseStream.Current);
            }

            return items;
        }

        public async Task SubscribeAsync(ObservableCollection<ToDoItemModel> items, CancellationToken ct = default)
        {
            using var call = _client.GetList(new Empty());

            try
            {
                while (await call.ResponseStream.MoveNext(ct))
                {
                    var item = call.ResponseStream.Current;
                    // update UI on UI thread
                    App.Current.Dispatcher.Invoke(() => {
                        var existing = items.FirstOrDefault(i => i.Id == item.Id);
                        if (existing == null)
                            items.Add(new ToDoItemModel { Id = item.Id, Description = item.Description, IsCompleted = item.Status == "Completed" });
                        else
                        {
                            existing.Description = item.Description;
                            existing.IsCompleted = item.Status == "Completed";
                        }
                    });
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                // subscription cancelled
            }
        }
    }
}