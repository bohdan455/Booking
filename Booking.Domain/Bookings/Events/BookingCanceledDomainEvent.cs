﻿using Booking.Domain.Abstractions;

namespace Booking.Domain.Bookings.Events;

public record BookingCanceledDomainEvent(Guid BookingId) : IDomainEvent;