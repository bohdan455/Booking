namespace Booking.Domain.Bookings;

public record DateRange(DateOnly Start, DateOnly End)
{
    public int LengthInDays = End.DayNumber - Start.DayNumber;

    public static DateRange Create(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            throw new ApplicationException("End date precedes start date");
        }

        return new DateRange(start, end);
    }
};