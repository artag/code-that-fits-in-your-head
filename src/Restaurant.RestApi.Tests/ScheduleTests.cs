using FsCheck;
using FsCheck.Xunit;

namespace Restaurant.RestApi.Tests;

public class ScheduleTests
{
    [Property]
    public Property Schedule()
    {
        return Prop.ForAll(
            GenReservation.ArrayOf().ToArbitrary(),
            ScheduleImp);
    }

    private static void ScheduleImp(Reservation[] reservations)
    {
        // Create a table for each reservation, to ensure that all
        // reservations can be allotted a table.
        var tables = reservations.Select(r => Table.Standard(r.Quantity));
        var sut = new MaitreD(
            TimeSpan.FromHours(18),
            TimeSpan.FromHours(21),
            TimeSpan.FromHours(6),
            tables);

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

        Assert.All(actual, o => AssertTables(tables, o.Value));
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
}
