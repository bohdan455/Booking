using Booking.Domain.Apartments;

namespace Booking.Domain.Bookings;

public record PricingDetails(Money PriceForPeriod, Money CleaningFee, Money AmenitiesUpCharge)
{
    public Money TotalPrice => PriceForPeriod + CleaningFee + AmenitiesUpCharge;
}