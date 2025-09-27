using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public async IAsyncEnumerable<ToDoItem> SubscribeAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var call = _client.GetList(new Empty());

            while (await call.ResponseStream.MoveNext(cancellationToken))
            {
                yield return call.ResponseStream.Current;
            }
        }
    }
}