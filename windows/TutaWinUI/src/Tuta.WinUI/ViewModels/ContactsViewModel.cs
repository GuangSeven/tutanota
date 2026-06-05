using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tuta.Core.Domain;
using Tuta.Core.Services;

namespace Tuta.WinUI.ViewModels;

public sealed partial class ContactsViewModel : ObservableObject
{
    private readonly IContactsService _contactsService;

    public ObservableCollection<Contact> Contacts { get; } = new();

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }

    public ContactsViewModel(IContactsService contactsService)
    {
        _contactsService = contactsService;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var contacts = await _contactsService.GetAllAsync(CancellationToken.None);
            Replace(Contacts, contacts);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SearchAsync()
    {
        IsBusy = true;
        try
        {
            var contacts = await _contactsService.SearchAsync(SearchQuery, CancellationToken.None);
            Replace(Contacts, contacts);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}
