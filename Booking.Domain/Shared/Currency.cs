﻿namespace Booking.Domain.Apartments;

public record Currency
{
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    internal static readonly Currency None = new("");
    
    private Currency(string code)
    {
        Code = code;
    }

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(x => x.Code == code) ??
               throw new ApplicationException("The currency code is invalid");
    }
    
    public static readonly IReadOnlyCollection<Currency> All =
    [
        Usd, Eur
    ];
};   