using Booking.Domain.Abstractions;
using Booking.Domain.Apartments;
using Booking.Domain.Bookings.Events;

namespace Booking.Domain.Bookings;

public sealed class Booking : Entity
{
    private Booking(Guid id, Guid apartmentId, Guid userId, DateRange duration, Money priceForPeriod, Money cleaningFee,
        Money amenitiesUpCharge, Money totalPrice, BookingStatus status, DateTime createdOnUtc) : base(id)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        CleaningFee = cleaningFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid ApartmentId { get; private set; }

    public Guid UserId { get; private set; }

    public DateRange Duration { get; private set; }

    public Money PriceForPeriod { get; private set; }

    public Money CleaningFee { get; private set; }

    public Money AmenitiesUpCharge { get; private set; }

    public Money TotalPrice { get; private set; }

    public BookingStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime? ConfirmedOnUtc { get; private set; }

    public DateTime? RejectedOnUtc { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public DateTime? CancelledOnUtc { get; private set; }

    public static Booking Reserve(Apartment apartment, Guid userId, DateRange duration, DateTime utcNow, PricingService pricingService)
    {
        var pricingDetails = pricingService.CalculatePrice(apartment, duration);
        
        var booking = new Booking(
            Guid.NewGuid(),
            apartment.Id,
            userId,
            duration,
            pricingDetails.TotalPrice,
            pricingDetails.CleaningFee,
            pricingDetails.AmenitiesUpCharge,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved, 
            utcNow); 
        
        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        apartment.LastBookedOnUtc = utcNow;

        return booking;
    }

    public Result Confirm(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
        {
            return Result.Failure(BookingErrors.NotReserved);
        }

        RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));
        
        Status = BookingStatus.Confirmed;
        ConfirmedOnUtc = utcNow;

        return Result.Success();
    }

    public Result Reject(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
        {
            return Result.Failure(BookingErrors.NotReserved);
        }

        RaiseDomainEvent(new BookingRejectedDomainEvent(Id));
        
        Status = BookingStatus.Rejected;
        ConfirmedOnUtc = utcNow;

        return Result.Success();
    }

    public Result Complete(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
        {
            return Result.Failure(BookingErrors.NotConfirmed);
        }
        
        RaiseDomainEvent(new BookingCompletedDomainEvent(Id));
        
        Status = BookingStatus.Completed;
        CompletedOnUtc = utcNow;
        
        return Result.Success();
    }

    public Result Cancel(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
        {
            return Result.Failure(BookingErrors.NotConfirmed);
        }

        var currentDate = DateOnly.FromDateTime(utcNow);

        if (currentDate > Duration.Start)
        {
            return Result.Failure(BookingErrors.AlreadyStarted);
        }

        RaiseDomainEvent(new BookingCanceledDomainEvent(Id));
        
        Status = BookingStatus.Canceled;
        CancelledOnUtc = utcNow;
        
        return Result.Success();
    }
}