using GalaSoft.MvvmLight.Command;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoListClientApp.Models;
using static Todo.ToDoService;

namespace ToDoListClientApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly Services.ToDoServiceClient _service;
        private readonly CancellationTokenSource _cts;

        public ObservableCollection<ToDoItemModel> Items { get; set; } = new();

        public string? NewDescription { get; set; }

        public ICommand AddCommand { get; }
        public ICommand ToggleStatusCommand { get; }

        public MainViewModel()
        {
            _service = new Services.ToDoServiceClient();
            AddCommand = new RelayCommand(async () => await AddItem());
            ToggleStatusCommand = new RelayCommand<ToDoItemModel>(async item => await ToggleStatus(item));
            Task.Run(LoadItems); 
            _cts = new CancellationTokenSource();
            Task.Run(() => _service.SubscribeAsync(Items, _cts.Token));
        }
        private async Task ToggleStatus(ToDoItemModel item)
        {
            if (item == null) return;

            var updated = await _service.ToggleStatusAsync(item.Id);

            item.IsCompleted = updated.Status == "Completed";
        }

        private async Task AddItem()
        {
            if (!string.IsNullOrWhiteSpace(NewDescription))
            {
                await _service.AddItemAsync(NewDescription);
            }
        }

        private async Task LoadItems()
        {
            Items.Clear();
            var response = await _service.GetListAsync();
            foreach (var item in response)
            {
                Items.Add(new ToDoItemModel
                {
                    Id = item.Id,
                    Description = item.Description,
                    IsCompleted = item.Status == "Completed"
                });
            }
        }
    }
}