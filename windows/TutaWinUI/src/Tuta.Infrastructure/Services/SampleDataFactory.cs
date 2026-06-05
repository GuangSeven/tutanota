using Tuta.Core.Domain;

namespace Tuta.Infrastructure.Services;

internal static class SampleDataFactory
{
    public static IReadOnlyList<Mailbox> Mailboxes() =>
    [
        new Mailbox("mbx-primary", "Primary", "user@tuta.example", true),
        new Mailbox("mbx-shared", "Shared", "team@tuta.example", false),
    ];

    public static IReadOnlyList<MailSummary> MailSummaries(string mailboxId) =>
    [
        new MailSummary("mail-001", mailboxId, "alice@tuta.example", "Welcome to Tuta", DateTimeOffset.UtcNow.AddHours(-2), "Encrypted onboarding guide...", true),
        new MailSummary("mail-002", mailboxId, "security@tuta.example", "Key rotation notice", DateTimeOffset.UtcNow.AddDays(-1), "Your key version has been updated...", false),
        new MailSummary("mail-003", mailboxId, "calendar@tuta.example", "Event invite", DateTimeOffset.UtcNow.AddDays(-3), "Meeting invite attached...", false),
    ];

    public static IReadOnlyList<MailMessage> MailMessages(string mailboxId) =>
    [
        new MailMessage(
            "mail-001",
            mailboxId,
            "alice@tuta.example",
            [new Recipient("user@tuta.example", "You")],
            [],
            [],
            "Welcome to Tuta",
            "Welcome to the new native client. This message is stored using local encryption.",
            DateTimeOffset.UtcNow.AddHours(-2),
            true),
        new MailMessage(
            "mail-002",
            mailboxId,
            "security@tuta.example",
            [new Recipient("user@tuta.example", "You")],
            [],
            [],
            "Key rotation notice",
            "Your account keys were rotated. Verify new fingerprints in Settings.",
            DateTimeOffset.UtcNow.AddDays(-1),
            true),
        new MailMessage(
            "mail-003",
            mailboxId,
            "calendar@tuta.example",
            [new Recipient("user@tuta.example", "You")],
            [],
            [],
            "Event invite",
            "You've been invited to the weekly sync.",
            DateTimeOffset.UtcNow.AddDays(-3),
            false),
    ];

    public static IReadOnlyList<Contact> Contacts() =>
    [
        new Contact("contact-001", "Alice", ["alice@tuta.example"], ["+49 30 123456"]),
        new Contact("contact-002", "Bob", ["bob@tuta.example"], ["+49 30 987654"]),
        new Contact("contact-003", "Security Team", ["security@tuta.example"], []),
    ];

    public static IReadOnlyList<CalendarEvent> CalendarEvents(string calendarId) =>
    [
        new CalendarEvent(
            "event-001",
            calendarId,
            "Weekly Sync",
            DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(9),
            DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(10),
            false,
            "Conference Room",
            "Discuss roadmap"),
        new CalendarEvent(
            "event-002",
            calendarId,
            "Release Review",
            DateTimeOffset.UtcNow.Date.AddDays(3).AddHours(14),
            DateTimeOffset.UtcNow.Date.AddDays(3).AddHours(15),
            false,
            "Online",
            "Security checklist"),
    ];
}
