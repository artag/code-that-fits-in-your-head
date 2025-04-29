using FsCheck;
using FsCheck.Xunit;

namespace Restaurant.RestApi.Tests;

public class ScheduleTests
{
    [Property]
    public Property Schedule()
    {
        return Prop.ForAll(
            GenReservation
                .ArrayOf()
                .SelectMany(rs => GenMaitreD(rs).Select(m => (m, rs)))
                .ToArbitrary(),
            t => ScheduleImp(t.m, t.rs));
    }

    private static void ScheduleImp(
        MaitreD sut,
        Reservation[] reservations)
    {
        var actual = sut.Schedule(reservations);

        Assert.NotNull(actual);
        Assert.Equal(
            reservations.Select(r => r.At).Distinct().Count(),
            actual.Count());
#pragma warning disable RCS1077 // Optimize LINQ method call
        Assert.Equal(
            actual.Select(o => o.At).OrderBy(d => d),
            actual.Select(o => o.At));
#pragma warning restore RCS1077 // Optimize LINQ method call

        Assert.All(actual, o => AssertTables(sut.Tables, o.Value));
    }

    private static void AssertTables(
        IEnumerable<Table> expected,
        IEnumerable<Table> actual)
    {
        var exp = expected.ToArray();
        var act = actual.ToArray();
        Assert.Equal(
            exp.Length,
            act.Length);
        Assert.Equal(
            exp.Sum(t => t.Capacity),
            act.Sum(t => t.Capacity));
    }

    private static Gen<Email> GenEmail =>
        from s in Arb.Default.NonWhiteSpaceString().Generator
        select new Email(s.Item);

    private static Gen<Name> GenName =>
        from s in Arb.Default.StringWithoutNullChars().Generator
        select new Name(s.Item);

    private static Gen<Reservation> GenReservation =>
        from id in Arb.Default.Guid().Generator
        from d in Arb.Default.DateTime().Generator
        from e in GenEmail
        from n in GenName
        from q in Arb.Default.PositiveInt().Generator
        select new Reservation(id, d, e, n, q.Item);

    private static Gen<MaitreD> GenMaitreD(
        IEnumerable<Reservation> reservations)
    {
        return
            from seatingDuration in Gen.Choose(1, 6)
            from tables in GenTables(reservations)
            select new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(seatingDuration),
                tables);
    }

    /// <summary>
    /// Generate a table configuration that can at minimum accomodate all
    /// reservations.
    /// </summary>
    /// <param name="reservations">The reservations to accommodate</param>
    /// <returns>A generator of valid table configurations.</returns>
    private static Gen<IEnumerable<Table>> GenTables(
        IEnumerable<Reservation> reservations)
    {
        // Create a table for each reservation, to ensure that all
        // reservations can be allotted a table.
        var tables = reservations.Select(r => Table.Standard(r.Quantity));
        return
            from moreTables in
                Gen.Choose(1, 12).Select(Table.Standard).ArrayOf()
            from allTables in Gen.Shuffle(tables.Concat(moreTables))
            select allTables.AsEnumerable();
    }
}
