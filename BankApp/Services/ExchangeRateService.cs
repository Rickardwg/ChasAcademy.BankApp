using BankApp.Enums;

namespace BankApp.Services;

internal class ExchangeRateService
{
    private readonly Dictionary<Currency, decimal> _ratesInSek = new()
    {
        [Currency.SEK] = 1m,
        [Currency.EUR] = 11.1m,
        [Currency.USD] = 9.9m

    };

    public bool TryGetRate(Currency source, Currency target, out decimal rate)
    {
        rate = default;

        if (!_ratesInSek.TryGetValue(source, out decimal sourceRate))
        {
            return false;
        }

        if (!_ratesInSek.TryGetValue(target, out decimal targetRate))
        {
            return false;
        }

        rate = sourceRate / targetRate;
        return true;
    }

    public bool TryUpdateRate(Currency currency, decimal sekPerUnit)
    {
        throw new NotImplementedException();
    }
}
