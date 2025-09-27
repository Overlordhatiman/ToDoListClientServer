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

        public ObservableCollection<ToDoItemModel> Items { get; set; } = new ObservableCollection<ToDoItemModel>();

        private string _newDescription;
        public string NewDescription
        {
            get => _newDescription;
            set { _newDescription = value; OnPropertyChanged(nameof(NewDescription)); }
        }

        public ICommand AddCommand { get; }
        public ICommand ToggleStatusCommand { get; }

        public MainViewModel()
        {
            _service = new Services.ToDoServiceClient();
            AddCommand = new RelayCommand(async () => await AddItem());
            ToggleStatusCommand = new RelayCommand<ToDoItemModel>(async item => await ToggleStatus(item));

            _cts = new CancellationTokenSource();
            _ = StartListeningAsync(_cts.Token);
        }
        private async Task ToggleStatus(ToDoItemModel item)
        {
            if (item == null) return;

            await _service.ToggleStatusAsync(item.Id);
        }

        private async Task AddItem()
        {
            if (string.IsNullOrWhiteSpace(NewDescription)) return;

            await _service.AddItemAsync(NewDescription);
            NewDescription = string.Empty;
        }
        private async Task StartListeningAsync(CancellationToken token)
        {
            await foreach (var item in _service.SubscribeAsync(token))
            {
                var existing = Items.FirstOrDefault(x => x.Id == item.Id);

                if (existing != null)
                {
                    existing.Description = item.Description;
                    existing.IsCompleted = item.Status == "Completed";
                }
                else
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
}