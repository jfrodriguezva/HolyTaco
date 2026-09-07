namespace HolyTac.Menu.Domain;

public readonly record struct Money(decimal Amount, string Currency = "MXN")
{
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("No se pueden sumar montos de distinta moneda.");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
