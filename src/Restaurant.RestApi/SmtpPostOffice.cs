using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Restaurant.RestApi;

public sealed class SmtpPostOffice : IPostOffice
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _fromAddress;

    public SmtpPostOffice(
        string host,
        int port,
        string userName,
        string password,
        string fromAddress)
    {
        _host = host;
        _port = port;
        _userName = userName;
        _password = password;
        _fromAddress = fromAddress;
    }

    public Task EmailReservationCreated(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        var subject = $"Your reservation for {reservation.Quantity}.";
        var body = CreateBodyForCreated(reservation);
        var email = reservation.Email.ToString();
        return Send(subject, body, email);
    }

    public Task EmailReservationDeleted(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        var subject = $"Your reservation for {reservation.Quantity} was cancelled.";
        var body = CreateBodyForDeleted(reservation);
        var email = reservation.Email.ToString();
        return Send(subject, body, email);
    }

    public Task EmailReservationUpdating(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        var subject = $"Your reservation for {reservation.Quantity} is changing.";
        var body = CreateBodyForUpdating(reservation);
        var email = reservation.Email.ToString();
        return Send(subject, body, email);
    }

    public Task EmailReservationUpdated(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        var subject = $"Your reservation for {reservation.Quantity} changed.";
        var body = CreateBodyForUpdated(reservation);
        var email = reservation.Email.ToString();
        return Send(subject, body, email);
    }

    public override bool Equals(object? obj)
    {
        return obj is SmtpPostOffice other &&
               _host == other._host &&
               _port == other._port &&
               _fromAddress == other._fromAddress &&
               _userName == other._userName &&
               _password == other._password;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            _host, _port, _fromAddress, _userName, _password);
    }

    private async Task Send(string subject, string body, string email)
    {
        using var msg = new MailMessage(_fromAddress, email);
        msg.Subject = subject;
        msg.Body = body;
        using var client = new SmtpClient();
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_userName, _password);
        client.Host = _host;
        client.Port = _port;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = true;
        await client.SendMailAsync(msg).ConfigureAwait(false);
    }

    private static string CreateBodyForCreated(Reservation reservation)
    {
        var sb = new StringBuilder();

        _ = sb.Append("Thank you for your reservation. ");
        _ = sb.AppendLine("Here's the details about your reservation:");
        _ = sb.AppendLine();
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"At: {reservation.At}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Party size: {reservation.Quantity}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Name: {reservation.Name}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Email: {reservation.Email}.");

        return sb.ToString();
    }

    private static string CreateBodyForDeleted(Reservation reservation)
    {
        var sb = new StringBuilder();

        _ = sb.Append("Your reservation was cancelled. ");
        _ = sb.AppendLine("Here's the details about your reservation:");
        _ = sb.AppendLine();
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"At: {reservation.At}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Party size: {reservation.Quantity}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Name: {reservation.Name}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Email: {reservation.Email}.");

        return sb.ToString();
    }

    private static string CreateBodyForUpdating(Reservation reservation)
    {
        var sb = new StringBuilder();

        _ = sb.Append("Your reservation is changing. ");
        _ = sb.AppendLine("Here's the details about your reservation:");
        _ = sb.AppendLine();
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"At: {reservation.At}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Party size: {reservation.Quantity}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Name: {reservation.Name}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Email: {reservation.Email}.");

        return sb.ToString();
    }

    private static string CreateBodyForUpdated(Reservation reservation)
    {
        var sb = new StringBuilder();

        _ = sb.Append("Your reservation changed. ");
        _ = sb.AppendLine("Here's the details about your reservation:");
        _ = sb.AppendLine();
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"At: {reservation.At}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Party size: {reservation.Quantity}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Name: {reservation.Name}.");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Email: {reservation.Email}.");

        return sb.ToString();
    }
}
