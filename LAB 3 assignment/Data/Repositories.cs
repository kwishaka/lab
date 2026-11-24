using LAB_3_assignment.Models;
using Microsoft.Data.Sqlite;

namespace LAB_3_assignment.Data;

public sealed class Repository
{
    public User? Authenticate(string username, string password)
    {
        using var c = Database.OpenConnection(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, Password, Role FROM Users WHERE Username=$u AND Password=$p";
        cmd.Parameters.AddWithValue("$u", username); cmd.Parameters.AddWithValue("$p", password);
        using var r = cmd.ExecuteReader();
        return r.Read() ? new User(r.GetInt32(0), r.GetString(1), r.GetString(2), Enum.Parse<UserRole>(r.GetString(3))) : null;
    }

    public List<User> GetUsers() => Query("SELECT Id, Username, Password, Role FROM Users ORDER BY Username", r => new User(r.GetInt32(0), r.GetString(1), r.GetString(2), Enum.Parse<UserRole>(r.GetString(3))));
    public void AddUser(string username, string password, UserRole role) => Execute("INSERT INTO Users(Username,Password,Role) VALUES($u,$p,$r)", (c, cmd) => { cmd.Parameters.AddWithValue("$u", username); cmd.Parameters.AddWithValue("$p", password); cmd.Parameters.AddWithValue("$r", role.ToString()); });
    public void DeleteUser(int id) => Execute("DELETE FROM Users WHERE Id=$id", (c, cmd) => cmd.Parameters.AddWithValue("$id", id));

    public List<Customer> GetCustomers(string search = "") => Query("SELECT Id,Name,Phone,IdentificationNumber FROM Customers WHERE Name LIKE $s OR Phone LIKE $s ORDER BY Name", r => new Customer(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3)), (cmd) => cmd.Parameters.AddWithValue("$s", $"%{search}%"));
    public void SaveCustomer(Customer customer)
    {
        if (customer.Id == 0) Execute("INSERT INTO Customers(Name,Phone,IdentificationNumber) VALUES($n,$p,$i)", (_, cmd) => AddCustomerParameters(cmd, customer));
        else Execute("UPDATE Customers SET Name=$n,Phone=$p,IdentificationNumber=$i WHERE Id=$id", (_, cmd) => { AddCustomerParameters(cmd, customer); cmd.Parameters.AddWithValue("$id", customer.Id); });
    }
    private static void AddCustomerParameters(SqliteCommand cmd, Customer c) { cmd.Parameters.AddWithValue("$n", c.Name); cmd.Parameters.AddWithValue("$p", c.Phone); cmd.Parameters.AddWithValue("$i", c.IdentificationNumber); }

    public List<Bicycle> GetBicycles() => Query("SELECT Id,Code,Type,Status,RentalCount FROM Bicycles ORDER BY Code", r => new Bicycle(r.GetInt32(0), r.GetString(1), r.GetString(2), Enum.Parse<BicycleStatus>(r.GetString(3)), r.GetInt32(4)));
    public List<Bicycle> GetAvailableBicycles() => GetBicycles().Where(b => b.Status == BicycleStatus.Available).ToList();
    public void SaveBicycle(Bicycle bicycle)
    {
        if (bicycle.Id == 0) Execute("INSERT INTO Bicycles(Code,Type,Status) VALUES($c,$t,$s)", (_, cmd) => AddBicycleParameters(cmd, bicycle));
        else Execute("UPDATE Bicycles SET Code=$c,Type=$t,Status=$s WHERE Id=$id", (_, cmd) => { AddBicycleParameters(cmd, bicycle); cmd.Parameters.AddWithValue("$id", bicycle.Id); });
    }
    public void DeleteBicycle(int id) => Execute("DELETE FROM Bicycles WHERE Id=$id AND Status <> 'Rented'", (c, cmd) => cmd.Parameters.AddWithValue("$id", id));
    private static void AddBicycleParameters(SqliteCommand cmd, Bicycle b) { cmd.Parameters.AddWithValue("$c", b.Code); cmd.Parameters.AddWithValue("$t", b.Type); cmd.Parameters.AddWithValue("$s", b.Status.ToString()); }

    public List<PriceBand> GetPrices() => Query("SELECT StartHour,EndHour,PricePerHour FROM PriceBands ORDER BY StartHour", r => new PriceBand(r.GetInt32(0), r.GetInt32(1), r.GetDecimal(2)));
    public void UpdatePrice(int startHour, decimal price) => Execute("UPDATE PriceBands SET PricePerHour=$p WHERE StartHour=$s", (_, cmd) => { cmd.Parameters.AddWithValue("$p", price); cmd.Parameters.AddWithValue("$s", startHour); });
    public int AddRental(int customerId, int bicycleId, int start, int end, decimal total)
    {
        using var c = Database.OpenConnection(); using var transaction = c.BeginTransaction();
        using var cmd = c.CreateCommand(); cmd.Transaction = transaction;
        cmd.CommandText = "INSERT INTO Rentals(CustomerId,BicycleId,StartHour,EndHour,TotalPrice,CreatedAt) VALUES($c,$b,$s,$e,$t,$d); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$c", customerId); cmd.Parameters.AddWithValue("$b", bicycleId); cmd.Parameters.AddWithValue("$s", start); cmd.Parameters.AddWithValue("$e", end); cmd.Parameters.AddWithValue("$t", total); cmd.Parameters.AddWithValue("$d", DateTime.Now.ToString("O"));
        var id = Convert.ToInt32(cmd.ExecuteScalar());
        using var update = c.CreateCommand(); update.Transaction = transaction; update.CommandText = "UPDATE Bicycles SET Status='Rented', RentalCount=RentalCount+1 WHERE Id=$id AND Status='Available'"; update.Parameters.AddWithValue("$id", bicycleId); update.ExecuteNonQuery();
        transaction.Commit(); return id;
    }
    public List<Rental> GetRentals(bool unpaidOnly = false) => Query($"SELECT r.Id,r.CustomerId,c.Name,r.BicycleId,b.Code,r.StartHour,r.EndHour,r.TotalPrice,r.IsPaid,r.CreatedAt FROM Rentals r JOIN Customers c ON c.Id=r.CustomerId JOIN Bicycles b ON b.Id=r.BicycleId {(unpaidOnly ? "WHERE r.IsPaid=0" : "")} ORDER BY r.CreatedAt DESC", r => new Rental(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetInt32(3), r.GetString(4), r.GetInt32(5), r.GetInt32(6), r.GetDecimal(7), r.GetBoolean(8), DateTime.Parse(r.GetString(9))));
    public void AddPayment(int rentalId, decimal amount, PaymentMethod method)
    {
        using var c = Database.OpenConnection(); using var transaction = c.BeginTransaction();
        using var cmd = c.CreateCommand(); cmd.Transaction = transaction; cmd.CommandText = "INSERT INTO Payments(RentalId,Amount,PaymentDate,Method) VALUES($r,$a,$d,$m)"; cmd.Parameters.AddWithValue("$r", rentalId); cmd.Parameters.AddWithValue("$a", amount); cmd.Parameters.AddWithValue("$d", DateTime.Now.ToString("O")); cmd.Parameters.AddWithValue("$m", method.ToString()); cmd.ExecuteNonQuery();
        using var paid = c.CreateCommand(); paid.Transaction = transaction; paid.CommandText = "UPDATE Rentals SET IsPaid=1 WHERE Id=$r AND (SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE RentalId=$r) >= TotalPrice"; paid.Parameters.AddWithValue("$r", rentalId); paid.ExecuteNonQuery(); transaction.Commit();
    }
    public void ReturnBicycle(int rentalId) => Execute("UPDATE Bicycles SET Status='Available' WHERE Id=(SELECT BicycleId FROM Rentals WHERE Id=$id) AND Status='Rented'", (_, cmd) => cmd.Parameters.AddWithValue("$id", rentalId));
    public decimal GetReceived(DateTime from, DateTime to) { using var c = Database.OpenConnection(); using var cmd = c.CreateCommand(); cmd.CommandText = "SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE PaymentDate >= $f AND PaymentDate < $t"; cmd.Parameters.AddWithValue("$f", from.ToString("O")); cmd.Parameters.AddWithValue("$t", to.ToString("O")); return Convert.ToDecimal(cmd.ExecuteScalar()); }

    private static List<T> Query<T>(string sql, Func<SqliteDataReader, T> map, Action<SqliteCommand>? parameters = null)
    { using var c = Database.OpenConnection(); using var cmd = c.CreateCommand(); cmd.CommandText = sql; parameters?.Invoke(cmd); using var r = cmd.ExecuteReader(); var result = new List<T>(); while (r.Read()) result.Add(map(r)); return result; }
    private static void Execute(string sql, Action<SqliteConnection, SqliteCommand> parameters) { using var c = Database.OpenConnection(); using var cmd = c.CreateCommand(); cmd.CommandText = sql; parameters(c, cmd); cmd.ExecuteNonQuery(); }
}
