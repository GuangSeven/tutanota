namespace Tuta.Core.Domain;

public sealed record Contact(
    string Id,
    string DisplayName,
    IReadOnlyList<string> EmailAddresses,
    IReadOnlyList<string> PhoneNumbers
)
{
    public string PrimaryEmail => EmailAddresses.Count > 0 ? EmailAddresses[0] : string.Empty;
}
