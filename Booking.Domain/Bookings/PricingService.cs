using Booking.Domain.Apartments;

namespace Booking.Domain.Bookings;

public class PricingService
{
    public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;
        var priceForPeriod = new Money(apartment.Price.Amount * period.LengthInDays, currency);
        
        var percentageUpCharge = apartment.Amenities.Sum(amenity => amenity switch
        {
            Amenity.GardenView or Amenity.MountainView => 0.05m,
            Amenity.AirConditioning or Amenity.Parking => 0.01m,
            _ => 0
        });
        
        var amenitiesUpCharge = percentageUpCharge > 0 
            ? new Money(priceForPeriod.Amount * percentageUpCharge, currency) 
            : Money.Zero();

        return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge);
    }
}