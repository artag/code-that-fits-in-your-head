namespace Restaurant.RestApi.Settings;

public class SmtpSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? FromAddress { get; set; }

    public IPostOffice ToPostOffice()
    {
        if (string.IsNullOrWhiteSpace(Host) ||
            string.IsNullOrWhiteSpace(UserName) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(FromAddress))
            return NullPostOffice.Instance;

        return new SmtpPostOffice(
            Host,
            Port,
            UserName,
            Password,
            FromAddress);
    }
}
