using Microsoft.Data.Sqlite;
using System;
using System.Globalization;

namespace Restaurant.RestApi;

public sealed class SqliteReservationsRepository : IReservationsRepository
{
    private readonly string _connectionString;

    public SqliteReservationsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task EnsureTables(CancellationToken ct = default)
    {
        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);
        await conn.OpenAsync(ct).ConfigureAwait(false);

        var cmd = new SqliteCommand(CreateReservationTableSql, conn);
        await using var disposeCmd = cmd.ConfigureAwait(false);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    public async Task Create(
        Reservation reservation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);
        await conn.OpenAsync(ct).ConfigureAwait(false);

        var cmd = new SqliteCommand(createReservationSql, conn);
        await using var disposeCmd2 = cmd.ConfigureAwait(false);
        cmd.Parameters.AddWithValue("@Id", reservation.Id);
        cmd.Parameters.AddWithValue("@At", reservation.At);
        cmd.Parameters.AddWithValue("@Name", reservation.Name.Value);
        cmd.Parameters.AddWithValue("@Email", reservation.Email.Value);
        cmd.Parameters.AddWithValue("@Quantity", reservation.Quantity);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Reservation>> ReadReservations(
        DateTime dateTime, CancellationToken ct = default)
    {
        var result = new List<Reservation>();

        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);
        await conn.OpenAsync(ct).ConfigureAwait(false);

        var cmd = new SqliteCommand(readByRangeSql, conn);
        await using var disposeCmd = cmd.ConfigureAwait(false);
        var at = dateTime.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        cmd.Parameters.AddWithValue("@At", at);

        var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        await using var disposeRdr = rdr.ConfigureAwait(false);
        while (await rdr.ReadAsync(ct).ConfigureAwait(false))
        {
            var publicIdStr = (string)rdr["PublicId"];
            var publicId = new Guid(publicIdStr);
            var timeAtStr = (string)rdr["At"];
            var timeAt = DateTime.Parse(timeAtStr, CultureInfo.InvariantCulture);
            var email = new Email((string)rdr["Email"]);
            var name = new Name((string)rdr["Name"]);
            var quantityLong = (long)rdr["Quantity"];
            var quantity = (int)quantityLong;

            var r = new Reservation(
                publicId,
                timeAt,
                email,
                name,
                quantity);

            result.Add(r);
        }

        return result.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Reservation>> ReadReservations(
        DateTime min,
        DateTime max,
        CancellationToken ct = default)
    {
        var result = new List<Reservation>();

        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);
        await conn.OpenAsync(ct).ConfigureAwait(false);

        var cmd = new SqliteCommand(readByRangeSql2, conn);
        await using var disposeCmd = cmd.ConfigureAwait(false);
        var minDate = min.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var maxDate = max.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        cmd.Parameters.AddWithValue("@Min", minDate);
        cmd.Parameters.AddWithValue("@Max", maxDate);

        var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        await using var disposeRdr = rdr.ConfigureAwait(false);
        while (await rdr.ReadAsync(ct).ConfigureAwait(false))
        {
            var publicIdStr = (string)rdr["PublicId"];
            var publicId = new Guid(publicIdStr);
            var timeAtStr = (string)rdr["At"];
            var timeAt = DateTime.Parse(timeAtStr, CultureInfo.InvariantCulture);
            var email = new Email((string)rdr["Email"]);
            var name = new Name((string)rdr["Name"]);
            var quantityLong = (long)rdr["Quantity"];
            var quantity = (int)quantityLong;

            var r = new Reservation(
                publicId,
                timeAt,
                email,
                name,
                quantity);

            result.Add(r);
        }

        return result.AsReadOnly();
    }

    public async Task<Reservation?> ReadReservation(
        Guid id, CancellationToken ct = default)
    {
        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);

        var cmd = new SqliteCommand(readByIdSql, conn);
        await using var disposeCmd = cmd.ConfigureAwait(false);
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync(ct).ConfigureAwait(false);
        var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        await using var disposeRdr = rdr.ConfigureAwait(false);

        var exists = await rdr.ReadAsync(ct).ConfigureAwait(false);
        if (!exists)
            return null;

        return CreateReservation(id, rdr);
    }

    public async Task Update(Reservation reservation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);

        var cmd = new SqliteCommand(updateSql, conn);
        await using var disposeCmd = cmd.ConfigureAwait(false);
        cmd.Parameters.AddWithValue("@Id", reservation.Id);
        cmd.Parameters.AddWithValue("@At", reservation.At);
        cmd.Parameters.AddWithValue("@Name", reservation.Name.Value);
        cmd.Parameters.AddWithValue("@Email", reservation.Email.Value);
        cmd.Parameters.AddWithValue("@Quantity", reservation.Quantity);

        await conn.OpenAsync(ct).ConfigureAwait(false);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    public async Task Delete(Guid id, CancellationToken ct = default)
    {
        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);

        var cmd = new SqliteCommand(deleteSql, conn);
        await using var disposeCmd = cmd.ConfigureAwait(false);
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync(ct).ConfigureAwait(false);
        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    private static Reservation? CreateReservation(Guid id, SqliteDataReader rdr)
    {
        var atStr = (string)rdr["At"];
        var name = (string)rdr["Name"];
        var email = (string)rdr["Email"];
        var quantityLong = (long)rdr["Quantity"];

        var at = DateTime.Parse(atStr, CultureInfo.InvariantCulture);
        var quantity = (int)quantityLong;

        return new Reservation(
            id,
            at,
            new Email(email),
            new Name(name),
            quantity);
    }

    public const string CreateReservationTableSql =
        @"
CREATE TABLE IF NOT EXISTS Reservations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    At NUMERIC NOT NULL,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    PublicId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT AK_PublicId UNIQUE(PublicId)
);
";

    private const string createReservationSql =
        @"
INSERT INTO Reservations (PublicId, At, Name, Email, Quantity)
VALUES (@Id, @At, @Name, @Email, @Quantity)
";

    private const string readByRangeSql =
        @"
SELECT PublicId, At, Name, Email, Quantity
FROM Reservations
WHERE DATE(At) = @At
";

    const string readByRangeSql2 =
        @"
SELECT PublicId, At, Name, Email, Quantity
FROM Reservations
WHERE @Min <= DATE(At) AND DATE(At) <= @Max
";

    // MSSQL
    //@"
    //SELECT PublicId, At, Name, Email, Quantity
    //FROM Reservations
    //WHERE CONVERT(DATE, At) = @At
    //";

    private const string readByIdSql =
        @"
SELECT At, Name, Email, Quantity
FROM Reservations
WHERE PublicId = @Id
";

    private const string updateSql =
        @"
UPDATE Reservations
SET [At]       = @At,
    [Name]     = @Name,
    [Email]    = @Email,
    [Quantity] = @Quantity
WHERE [PublicId] = @Id
";

    private const string deleteSql =
        @"
DELETE FROM Reservations
WHERE PublicId = @Id
";
}
