﻿namespace Booking.Domain.Apartments;

public record Money(decimal Amount, Currency Currency)
{
    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency || second.IsZero())
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return first with { Amount = first.Amount + second.Amount };
    }

    public static Money Zero() => new(0, Currency.None);
    public static Money Zero(Currency currency) => new(0, currency);

    public bool IsZero()
    {
        return this == Zero(Currency);
    }
}