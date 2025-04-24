namespace Restaurant.RestApi;

internal interface IPeriod
{
    T Accept<T>(IPeriodVisitor<T> visitor);
}
