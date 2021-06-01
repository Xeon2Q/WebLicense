namespace WebLicense.Shared.Customers
{
    public sealed record CustomerUserInfo
    {
        public long Id { get; init; }

        public string Name { get; init; }

        public string Email { get; init; }
    }
}