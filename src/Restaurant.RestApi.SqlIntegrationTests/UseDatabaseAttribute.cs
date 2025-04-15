using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace Restaurant.RestApi.SqlIntegrationTests;

public sealed class UseDatabaseAttribute : BeforeAfterTestAttribute
{
    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "No user input, but resource stream.")]

    public override void Before(MethodInfo methodUnderTest)
    {
        DeleteDatabase();

        using var conn = new SqliteConnection(ConnectionStrings.Reservations);
        conn.Open();
        const string createCmd = SqliteReservationsRepository.CreateReservationTableSql;
        using var cmd = new SqliteCommand(createCmd, conn);
        cmd.ExecuteNonQuery();

        // IN MSSQL example in book.
        //var builder = new SqliteConnectionStringBuilder(
        //    ConnectionStrings.Reservations);
        //using var conn = new SqliteConnection(builder.ConnectionString);
        //using var cmd = new SqliteCommand();
        //conn.Open();
        //cmd.Connection = conn;

        //foreach (var sql in ReadSchema())
        //{
        //    cmd.CommandText = sql;
        //    cmd.ExecuteNonQuery();
        //}

        base.Before(methodUnderTest);
    }

    // IN MSSQL example in book.
    //private static IEnumerable<string> ReadSchema()
    //{
    //    yield return "CREATE DATABASE [RestaurantIntegrationTest]";
    //    yield return "USE [RestaurantIntegrationTest]";

    //    var dbSchema = ReadDdl("RestaurantDbSchema");
    //    foreach (var s in SeperateStatements(dbSchema))
    //        yield return s;

    //    var addGuidColumn = ReadDdl("AddGuidColumnToReservations");
    //    foreach (var s in SeperateStatements(addGuidColumn))
    //        yield return s;
    //}

    //private static string ReadDdl(string name)
    //{
    //    using var strm = typeof(SqliteReservationsRepository)
    //        .Assembly
    //        .GetManifestResourceStream(
    //            $"Ploeh.Samples.Restaurant.RestApi.{name}.sql");
    //    using var rdr = new StreamReader(strm!);
    //    return rdr.ReadToEnd();
    //}

    //private static string[] SeperateStatements(string schemaSql)
    //{
    //    var separator = new[] { "GO" };
    //    return schemaSql.Split(
    //        separator,
    //        StringSplitOptions.RemoveEmptyEntries);
    //}

    public override void After(MethodInfo methodUnderTest)
    {
        base.After(methodUnderTest);
        DeleteDatabase();
    }

    private static void DeleteDatabase()
    {
        const string dropCmd = "DROP TABLE IF EXISTS Reservations";
        using var conn = new SqliteConnection(ConnectionStrings.Reservations);
        conn.Open();
        using var cmd = new SqliteCommand(dropCmd, conn);
        cmd.ExecuteNonQuery();

        // IN MSSQL example in book.
        //const string dropCmd = @"
        //        IF EXISTS (SELECT name
        //            FROM master.dbo.sysdatabases
        //            WHERE name = N'RestaurantIntegrationTest')
        //        DROP DATABASE[RestaurantIntegrationTest];";

        //var builder = new SqliteConnectionStringBuilder(
        //    ConnectionStrings.Reservations);
        //using var conn = new SqliteConnection(builder.ConnectionString);
        //using var cmd = new SqliteCommand(dropCmd, conn);
        //conn.Open();
        //cmd.ExecuteNonQuery();
    }
}
