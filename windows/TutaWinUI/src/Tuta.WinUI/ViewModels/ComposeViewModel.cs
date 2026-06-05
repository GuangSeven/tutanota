using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tuta.Core.Domain;
using Tuta.Core.Services;

namespace Tuta.WinUI.ViewModels;

public sealed partial class ComposeViewModel : ObservableObject
{
    private readonly IMailComposeService _composeService;
    private readonly IMailboxService _mailboxService;

    [ObservableProperty]
    private string to = string.Empty;

    [ObservableProperty]
    private string subject = string.Empty;

    [ObservableProperty]
    private string body = string.Empty;

    [ObservableProperty]
    private bool isConfidential = true;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string status = string.Empty;

    public IAsyncRelayCommand SendCommand { get; }
    public IAsyncRelayCommand SaveDraftCommand { get; }

    public ComposeViewModel(IMailComposeService composeService, IMailboxService mailboxService)
    {
        _composeService = composeService;
        _mailboxService = mailboxService;
        SendCommand = new AsyncRelayCommand(SendAsync);
        SaveDraftCommand = new AsyncRelayCommand(SaveDraftAsync);
    }

    private async Task SendAsync()
    {
        IsBusy = true;
        Status = string.Empty;
        try
        {
            var mailboxId = await ResolveMailboxIdAsync();
            var draft = new MailDraft(
                mailboxId,
                ParseRecipients(To),
                [],
                [],
                Subject,
                Body,
                IsConfidential,
                null);

            await _composeService.SendAsync(draft, CancellationToken.None);
            Status = "Message queued for send.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveDraftAsync()
    {
        IsBusy = true;
        Status = string.Empty;
        try
        {
            var mailboxId = await ResolveMailboxIdAsync();
            var draft = new MailDraft(
                mailboxId,
                ParseRecipients(To),
                [],
                [],
                Subject,
                Body,
                IsConfidential,
                null);

            await _composeService.SaveDraftAsync(draft, CancellationToken.None);
            Status = "Draft saved locally.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<string> ResolveMailboxIdAsync()
    {
        var mailboxes = await _mailboxService.GetMailboxesAsync(CancellationToken.None);
        return mailboxes.FirstOrDefault(m => m.IsPrimary)?.Id ?? mailboxes.First().Id;
    }

    private static IReadOnlyList<Recipient> ParseRecipients(string input)
    {
        var parts = input.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Select(p => new Recipient(p, p)).ToList();
    }
}
