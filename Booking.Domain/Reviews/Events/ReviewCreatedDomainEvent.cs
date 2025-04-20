using Booking.Domain.Abstractions;

namespace Booking.Domain.Reviews.Events;

public record ReviewCreatedDomainEvent(Guid ReviewId) : IDomainEvent;