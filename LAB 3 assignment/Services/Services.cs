using LAB_3_assignment.Data;
using LAB_3_assignment.Models;

namespace LAB_3_assignment.Services;

public sealed class PriceService(Repository repository)
{
    public decimal Calculate(int start, int end) => Enumerable.Range(start, end - start).Sum(hour => repository.GetPrices().First(p => hour >= p.StartHour && hour < p.EndHour).PricePerHour);
}

public sealed class RentalService(Repository repository, PriceService prices)
{
    public int CreateRental(int customerId, int bicycleId, string startText, string endText, out decimal total)
    {
        if (!RentalValidation.TryValidate(startText, endText, out var start, out var end, out var error)) throw new ArgumentException(error);
        total = prices.Calculate(start, end);
        return repository.AddRental(customerId, bicycleId, start, end, total);
    }
}
