namespace Restaurant.RestApi;

internal interface IPeriodVisitor<out T>
{
    T VisitYear(int year);
    T VisitMonth(int year, int month);
    T VisitDay(int year, int month, int day);
}
