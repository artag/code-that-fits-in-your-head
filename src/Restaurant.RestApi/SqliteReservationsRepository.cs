using Microsoft.Data.Sqlite;

namespace Restaurant.RestApi;

public class SqliteReservationsRepository : IReservationsRepository
{
    private readonly string _connectionString;

    public SqliteReservationsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task Create(
        Reservation reservation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        var conn = new SqliteConnection(_connectionString);
        await using var disposeConn = conn.ConfigureAwait(false);
        await conn.OpenAsync(ct).ConfigureAwait(false);

        var cmd1 = new SqliteCommand(createReservationTableSql, conn);
        await using (var disposeCmd1 = cmd1.ConfigureAwait(false))
        {
            await cmd1.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        var cmd2 = new SqliteCommand(createReservationSql, conn);
        await using var disposeCmd2 = cmd2.ConfigureAwait(false);
        cmd2.Parameters.Add(new SqliteParameter("@At", reservation.At));
        cmd2.Parameters.Add(new SqliteParameter("@Name", reservation.Name));
        cmd2.Parameters.Add(new SqliteParameter("@Email", reservation.Email));
        cmd2.Parameters.Add(new SqliteParameter("@Quantity", reservation.Quantity));
        await cmd2.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
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
                (DateTime)rdr["At"],
                (string)rdr["Name"],
                (string)rdr["Email"],
                (int)rdr["Quantity"]);

            result.Add(r);
        }

        return result.AsReadOnly();
    }

    private const string createReservationTableSql =
@"
CREATE TABLE IF NOT EXISTS Reservations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    At NUMERIC NOT NULL,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL,
    Quantity INTEGER NOT NULL
);
";

    private const string createReservationSql =
@"
INSERT INTO Reservations (At, Name, Email, Quantity)
VALUES (@At, @Name, @Email, @Quantity)
";

    private const string readByRangeSql =
@"
SELECT At, Name, Email, Quantity
FROM Reservations
WHERE CONVERT(DATE, At) = @At
";
}
