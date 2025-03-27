using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Restaurant.RestApi;

internal sealed class SqliteReservationsRepository : IReservationsRepository
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

        var cmd = new SqliteCommand(createReservationTableSql, conn);
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
        cmd.Parameters.Add(new SqliteParameter("@Id", reservation.Id));
        cmd.Parameters.Add(new SqliteParameter("@At", reservation.At));
        cmd.Parameters.Add(new SqliteParameter("@Name", reservation.Name));
        cmd.Parameters.Add(new SqliteParameter("@Email", reservation.Email));
        cmd.Parameters.Add(new SqliteParameter("@Quantity", reservation.Quantity));
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
        cmd.Parameters.AddWithValue("@At", dateTime.Date);

        var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        await using var disposeRdr = rdr.ConfigureAwait(false);
        while (await rdr.ReadAsync(ct).ConfigureAwait(false))
        {
            var r = new Reservation(
                (Guid)rdr["PublicId"],
                (DateTime)rdr["At"],
                (string)rdr["Name"],
                (string)rdr["Email"],
                (int)rdr["Quantity"]);

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
            name,
            email,
            quantity);
    }

    private const string createReservationTableSql =
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

    private const string deleteSql =
        @"
DELETE FROM Reservations
WHERE PublicId = @Id
";
}
