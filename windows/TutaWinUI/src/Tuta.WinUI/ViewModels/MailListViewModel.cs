using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tuta.Core.Domain;
using Tuta.Core.Events;
using Tuta.Core.Services;

namespace Tuta.WinUI.ViewModels;

public sealed partial class MailListViewModel : ObservableObject
{
    private readonly IMailboxService _mailboxService;
    private readonly IMailService _mailService;
    private readonly IEntityUpdateHub _updates;
    private string? _mailboxId;

    public ObservableCollection<MailSummary> Messages { get; } = new();

    [ObservableProperty]
    private MailSummary? selectedMessage;

    [ObservableProperty]
    private MailMessage? selectedMessageDetail;

    [ObservableProperty]
    private bool isBusy;

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    public string SelectedSubject => SelectedMessageDetail?.Subject ?? "Select a message";
    public string SelectedFrom => SelectedMessageDetail?.From ?? string.Empty;
    public string SelectedBody => SelectedMessageDetail?.Body ?? string.Empty;

    public MailListViewModel(IMailboxService mailboxService, IMailService mailService, IEntityUpdateHub updates)
    {
        _mailboxService = mailboxService;
        _mailService = mailService;
        _updates = updates;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        _updates.EntityUpdated += OnEntityUpdated;
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var mailboxes = await _mailboxService.GetMailboxesAsync(CancellationToken.None);
            var primary = mailboxes.FirstOrDefault(m => m.IsPrimary) ?? mailboxes.FirstOrDefault();
            if (primary == null)
            {
                Messages.Clear();
                SelectedMessageDetail = null;
                return;
            }

            _mailboxId = primary.Id;
            var inbox = await _mailService.GetInboxAsync(primary.Id, CancellationToken.None);
            Replace(Messages, inbox);
            SelectedMessageDetail = null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnEntityUpdated(object? sender, EntityUpdate update)
    {
        if (update.EntityType == "MailMessage")
        {
            _ = LoadAsync();
        }
    }

    partial void OnSelectedMessageChanged(MailSummary? value)
    {
        _ = LoadSelectedAsync(value);
    }

    partial void OnSelectedMessageDetailChanged(MailMessage? value)
    {
        OnPropertyChanged(nameof(SelectedSubject));
        OnPropertyChanged(nameof(SelectedFrom));
        OnPropertyChanged(nameof(SelectedBody));
    }

    private async Task LoadSelectedAsync(MailSummary? summary)
    {
        if (summary == null || _mailboxId == null)
        {
            SelectedMessageDetail = null;
            return;
        }

        SelectedMessageDetail = await _mailService.GetMessageAsync(_mailboxId, summary.Id, CancellationToken.None);
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
