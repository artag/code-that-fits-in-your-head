﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi.Tests;

public class ReservationsTests
{
    [Fact]
    public async Task PostValidReservation()
    {
        var at = CreateAt("19:00");
        await using var service = new LegacyApi();
        var expected = new ReservationDto
        {
            At = at,
            Email = "katinka@example.com",
            Name = "Katinka Ingabogovinanana",
            Quantity = 2
        };
        var response = await service.PostReservation(expected);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}."
            );
        var actual = await response.ParseJsonContent<ReservationDto>();
        Assert.Equal(expected, actual, new ReservationDtoComparer()!);
    }

    [Theory]
    [InlineData("19:00", "juliad@example.net", "Julia Domna", 5)]
    [InlineData("18:15", "x@example.com", "Xenia Ng", 9)]
    [InlineData("16:55", "kite@example.edu", null, 2)]
    [InlineData("17:30", "shli@example.org", "Shanghai Li", 5)]
    public async Task PostValidReservationWhenDatabaseIsEmpty(
        string time, string email, string name, int quantity)
    {
        var at = CreateAt(time);
        var db = new FakeDatabase();
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dto = new ReservationDto
        {
            Id = "59D553DA-F106-42ED-864C-814F25E8753A",
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity,
        };

        await sut.Post(dto);

        var expected = new SpyPostOffice.Observation(
            SpyPostOffice.Event.Created,
            new Reservation(
                Guid.Parse(dto.Id),
                DateTime.Parse(dto.At, CultureInfo.InvariantCulture),
                new Email(dto.Email),
                new Name(dto.Name ?? string.Empty),
                dto.Quantity));
        Assert.Contains(expected.Reservation, db.Grandfather);
        Assert.Contains(expected, postOffice);
    }

    [Theory]
    [InlineData(null, "j@example.net", "Jay Xerxes", 1)]
    [InlineData("not a date", "w@example.edu", "Wk Hd", 8)]
    [InlineData("2023-11-30 20:01", null, "Wk Hd", 19)]
    [InlineData("2022-01-02 12:10", "3@example.org", "3 Beard", 0)]
    [InlineData("2045-12-31 11:45", "git@example.com", "Gil Tan", -1)]
    public async Task PostInvalidReservation(
        string at, string email, string name, int quantity)
    {
        await using var service = new LegacyApi();
        var response = await service.PostReservation(new { at, email, name, quantity });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task OverbookAttempt()
    {
        var now = DateTime.Now.AddDays(1);
        await using var service = new LegacyApi();
        await service.PostReservation(
            new
            {
                at = now,
                email = "mars@example.edu",
                name = "Marina Seminova",
                quantity = 6,
            });

        var response = await service.PostReservation(
            new
            {
                at = now,
                email = "shli@example.org",
                name = "Shanghai Li",
                quantity = 5,
            });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(response.Content);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("tables", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task BoolTableWhenFreeSeatingIsAvailable()
    {
        var at = CreateAt("18:00");
        await using var service = new LegacyApi();
        await service.PostReservation(
            new
            {
                at = at,
                email = "net@example.net",
                name = "Ned Tucker",
                quantity = 2
            });

        var response = await service.PostReservation(
            new
            {
                at = at,
                email = "kant@example.edu",
                name = "Katrine Troelsen",
                quantity = 4
            });

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
    }

    [Theory]
    [InlineData("19:10", "adur@example.net", "Adrienne Ursa", 2)]
    [InlineData("18:55", "emol@example.gov", "Emma Olsen", 5)]
    public async Task ReadSuccessfulReservation(
        string time, string email, string name, int quantity)
    {
        var at = CreateAt(time);
        await using var service = new LegacyApi();
        var expected = new ReservationDto
        {
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity
        };
        var postResp = await service.PostReservation(expected);
        var address = FindReservationAddress(postResp);

        var getResp = await service.CreateClient().GetAsync(address);

        Assert.True(
            getResp.IsSuccessStatusCode,
            $"Actual status code: {postResp.StatusCode}.");
        var actual = await getResp.ParseJsonContent<ReservationDto>();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual, new ReservationDtoComparer());
        Assert.DoesNotContain(
            address!.GetLeftPart(UriPartial.Path),
            char.IsUpper);
    }

    [SuppressMessage(
        "Usage",
        "CA2234:Pass system uri objects instead of strings",
        Justification = "URL isn't passed as variable, but as literal.")]
    [Theory]
    [InlineData("3F749AB6-2E1C-4CAE-9988-7CA708DB9252")]
    [InlineData("foo")]
    public async Task GetAbsentReservation(string id)
    {
        await using var service = new LegacyApi();
        var client = service.CreateClient();

        var resp = await client.GetAsync($"/reservations/{id}");

        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Theory]
    [InlineData("DA5EFE7FAE914F43828467D403DD9814")]
    [InlineData("d4fec75f8d054299975515f757f1223e")]
    public async Task NoHackingOfUrlsAllowed(string id)
    {
        await using var service = new LegacyApi();
        var dto = Some.Reservation.ToDto();
        dto.Id = id;
        var postResp = await service.PostReservation(dto);
        postResp.EnsureSuccessStatusCode();

        /* This is the sort of 'hacking' of URLs that clients should be
         * discouraged from. Clients should be following links, as all the
         * other tests in this test suite demonstrate. */
        var urlHack = new Uri($"/reservations/{id}", UriKind.Relative);
        var getResp = await service.CreateClient().GetAsync(urlHack);

        /* The expected result of a 'hacked' URL is 404 Not Found rather
         * than 403 Forbidden. Clients are simply requesting a URL which
         * doesn't exist. */
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    [Theory]
    [InlineData("18:47", "c@example.net", "Nick Klimenko", 2)]
    [InlineData("18:50", "emot@example.gov", "Emma Otting", 2)]
    public async Task DeleteReservation(
        string time,
        string email,
        string name,
        int quantity)
    {
        var at = CreateAt(time);
        await using var service = new LegacyApi();
        var dto = new ReservationDto
        {
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity
        };
        var postRep = await service.PostReservation(dto);
        var address = FindReservationAddress(postRep);
        var client = service.CreateClient();

        var deleteResp = await client.DeleteAsync(address);

        Assert.True(
            deleteResp.IsSuccessStatusCode,
            $"Actual status code: {deleteResp.StatusCode}.");
        var getResp = await client.GetAsync(address);
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    [Theory]
    [InlineData("d46668b1ea484d9d918e2143e2f6e991")]
    [InlineData("25FBFF44-2837-4772-91D2-3BAC7CA1DB4C")]
    public async Task DeleteIsIdempotent(string id)
    {
        await using var service = new LegacyApi();
        var dto = Some.Reservation.ToDto();
        dto.Id = id;
        var postResp = await service.PostReservation(dto);
        postResp.EnsureSuccessStatusCode();
        var url = FindReservationAddress(postResp);

        var resp = await service.CreateClient().DeleteAsync(url);

        Assert.True(
            resp.IsSuccessStatusCode,
            $"Actual status code: {resp.StatusCode}.");
    }

    [Fact]
    public async Task DeleteSendsEmail()
    {
        var r = Some.Reservation;
        var db = new FakeDatabase();
        db.Grandfather.Add(r);
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        await sut.Delete(r.Id.ToString("N"));

        var expected = new SpyPostOffice.Observation(
            SpyPostOffice.Event.Deleted,
            r);
        Assert.Contains(expected, postOffice);
    }

    [Fact]
    public async Task DeleteAbsentReservationDoesNotSendEmail()
    {
        var db = new FakeDatabase();
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        await sut.Delete(Guid.NewGuid().ToString("N"));

        Assert.DoesNotContain(
            postOffice,
            o => o.Event == SpyPostOffice.Event.Deleted);
    }

    [Theory]
    [InlineData("18:47", "b@example.net", "Bjork", 2, 5)]
    [InlineData("19:32", "e@example.gov", "Epica", 5, 4)]
    public async Task UpdateReservation(
        string time,
        string email,
        string name,
        int quantity,
        int newQuantity)
    {
        var at = CreateAt(time);
        await using var service = new LegacyApi();
        var dto = new ReservationDto
        {
            At = at,
            Email = email,
            Name = name,
            Quantity = quantity
        };
        var postResp = await service.PostReservation(dto);
        var address = FindReservationAddress(postResp);

        dto.Quantity = newQuantity;
        var putResp = await service.PutReservation(address!, dto);

        Assert.True(
            putResp.IsSuccessStatusCode,
            $"Actual status code: {putResp.StatusCode}");
        using var client = service.CreateClient();
        var getResp = await client.GetAsync(address);
        var persisted = await getResp.ParseJsonContent<ReservationDto>();
        Assert.Equal(dto, persisted, new ReservationDtoComparer()!);
        var actual = await putResp.ParseJsonContent<ReservationDto>();
        Assert.NotNull(actual);
        Assert.Equal(persisted, actual, new ReservationDtoComparer()!);
    }

    [Theory]
    [InlineData(null, "led@example.net", "Light Expansion Dread", 2)]
    [InlineData("not a date", "cygnet@example.edu", "Committee", 9)]
    [InlineData("19:00", null, "Quince", 3)]
    [InlineData("19:10", "4@example.org", "4 Beard", 0)]
    [InlineData("18:45", "svn@example.com", "Severin", -1)]
    public async Task PutInvalidReservation(
        string time,
        string email,
        string name,
        int quantity)
    {
        await using var service = new LegacyApi();
        var dto = new ReservationDto
        {
            At = CreateAt("19:00"),
            Email = "soylent@example.net",
            Name = ":wumpscut:",
            Quantity = 1
        };
        var postResp = await service.PostReservation(dto);
        postResp.EnsureSuccessStatusCode();
        var address = FindReservationAddress(postResp);

        var at = CreateAt(time);
        var putResp = await service.PutReservation(
            address!,
            new { at, email, name, quantity });

        Assert.Equal(HttpStatusCode.BadRequest, putResp.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("bas")]
    public async Task PutInvalidId(string invalidId)
    {
        var db = new FakeDatabase();
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dummyDto = new ReservationDto
        {
            At = CreateAt("18:19"),
            Email = "thorne@example.com",
            Name = "Tracy Thorne",
            Quantity = 2
        };
        var actual = await sut.Put(invalidId, dummyDto);

        Assert.IsAssignableFrom<NotFoundResult>(actual);
    }

    [Fact]
    public async Task PutConflictingIds()
    {
        var db = new FakeDatabase();
        db.Grandfather.Add(Some.Reservation);
        var postOffice = new SpyPostOffice();
        var dt = new DateTime(2022, 4, 1, 19, 15, 0);
        var dateTimeService = new SpyDateTimeService(dt);
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dto = Some.Reservation
            .WithId(Guid.NewGuid())
            .WithName(new Name("Qux"))
            .ToDto();
        var id = Some.Reservation.Id.ToString("N");
        await sut.Put(id, dto);

        var r = Assert.Single(db.Grandfather);
        Assert.Equal(Some.Reservation.WithName(new Name("Qux")), r);
    }

    [Fact]
    public async Task PutAbsentReservation()
    {
        var db = new FakeDatabase();
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dto = new ReservationDto
        {
            At = CreateAt("18:21"),
            Email = "tori@example.org",
            Name = "Tori Amos",
            Quantity = 9
        };
        var id = Guid.NewGuid().ToString("N");
        var actual = await sut.Put(id, dto);

        Assert.IsAssignableFrom<NotFoundResult>(actual);
    }

    [Fact]
    public async Task ChangeDateToSoldOutDate()
    {
        var r1 = Some.Reservation;
        var r2 = Some.Reservation
            .WithId(Guid.NewGuid())
            .TheDayAfter()
            .WithQuantity(10);
        var db = new FakeDatabase();
        db.Grandfather.Add(r1);
        db.Grandfather.Add(r2);
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dto = r1.WithDate(r2.At).ToDto();
        var actual = await sut.Put(r1.Id.ToString("N"), dto);

        var oRes = Assert.IsAssignableFrom<ObjectResult>(actual);
        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            oRes.StatusCode);
    }

    [Fact]
    public async Task EditReservationOnSameDayNearCapacity()
    {
        const string time = "20:01";
        var at = CreateAt(time);

        await using var service = new LegacyApi();
        var dto = new ReservationDto
        {
            At = at,
            Email = "aol@example.gov",
            Name = "Anette Olzon",
            Quantity = 5
        };
        var postResp = await service.PostReservation(dto);
        postResp.EnsureSuccessStatusCode();
        var address = FindReservationAddress(postResp!);

        dto.Quantity++;
        var putResp = await service.PutReservation(address!, dto);

        Assert.True(
            putResp.IsSuccessStatusCode,
            $"Actual status code: {putResp.StatusCode}.");
    }

    [Theory]
    [InlineData("ploeh")]
    [InlineData("fnaah")]
    public async Task PutSendsEmail(string newName)
    {
        var r = Some.Reservation;
        var db = new FakeDatabase();
        db.Grandfather.Add(r);
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dto = r.WithName(new Name(newName)).ToDto();
        await sut.Put(r.Id.ToString("N"), dto);

        var expected = new SpyPostOffice.Observation(
            SpyPostOffice.Event.Updated,
            r.WithName(new Name(newName)));
        Assert.Contains(expected, postOffice);
        Assert.DoesNotContain(
            postOffice,
            o => o.Event == SpyPostOffice.Event.Updating);
    }

    [Theory]
    [InlineData("foo@example.com")]
    [InlineData("bar@example.gov")]
    public async Task PutSendsEmailToOldAddressOnChange(string newEmail)
    {
        var r = Some.Reservation;
        var db = new FakeDatabase();
        db.Grandfather.Add(r);
        var postOffice = new SpyPostOffice();
        var dateTimeService = new SpyDateTimeService(new DateTime(2022, 03, 31, 19, 37, 45));
        var sut = new ReservationsController(db, postOffice, dateTimeService, Some.MaitreD);

        var dto = r.WithEmail(new Email(newEmail)).ToDto();
        await sut.Put(r.Id.ToString("N"), dto);

        var expected = new[] {
            new SpyPostOffice.Observation(
                SpyPostOffice.Event.Updating,
                r),
            new SpyPostOffice.Observation(
                SpyPostOffice.Event.Updated,
                r.WithEmail(new Email(newEmail))) }.ToHashSet();
        Assert.Superset(expected, postOffice.ToHashSet());
    }

    private static Uri? FindReservationAddress(HttpResponseMessage response)
    {
        return response.Headers.Location;
    }

    private static string CreateAt(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            return time;

        var parsed = DateTime.TryParse(time, out var dt);
        if (!parsed)
            return time;

        var dateNow = DateTime.Now.Date.AddDays(1);
        var timeAt = TimeSpan.Parse(time, CultureInfo.InvariantCulture);
        return dateNow.Add(timeAt).ToString("O");
    }
}
