using Microsoft.Data.Sqlite;

namespace LAB_3_assignment.Data;

public static class Database
{
    private static readonly string DatabasePath = Path.Combine(AppContext.BaseDirectory, "bicycle-rental.db");
    public static string ConnectionString => $"Data Source={DatabasePath}";

    public static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static void Initialize()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            PRAGMA foreign_keys = ON;
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL, Role TEXT NOT NULL);
            CREATE TABLE IF NOT EXISTS Customers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Phone TEXT NOT NULL UNIQUE,
                IdentificationNumber TEXT NOT NULL UNIQUE);
            CREATE TABLE IF NOT EXISTS Bicycles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, Code TEXT NOT NULL UNIQUE, Type TEXT NOT NULL,
                Status TEXT NOT NULL DEFAULT 'Available', RentalCount INTEGER NOT NULL DEFAULT 0);
            CREATE TABLE IF NOT EXISTS PriceBands (
                StartHour INTEGER PRIMARY KEY, EndHour INTEGER NOT NULL, PricePerHour REAL NOT NULL);
            CREATE TABLE IF NOT EXISTS Rentals (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, CustomerId INTEGER NOT NULL, BicycleId INTEGER NOT NULL,
                StartHour INTEGER NOT NULL, EndHour INTEGER NOT NULL, TotalPrice REAL NOT NULL,
                IsPaid INTEGER NOT NULL DEFAULT 0, CreatedAt TEXT NOT NULL,
                FOREIGN KEY(CustomerId) REFERENCES Customers(Id), FOREIGN KEY(BicycleId) REFERENCES Bicycles(Id));
            CREATE TABLE IF NOT EXISTS Payments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, RentalId INTEGER NOT NULL, Amount REAL NOT NULL,
                PaymentDate TEXT NOT NULL, Method TEXT NOT NULL,
                FOREIGN KEY(RentalId) REFERENCES Rentals(Id));
            INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES
                ('admin', 'admin123', 'Administrator'), ('attendant', 'attendant123', 'Attendant');
            INSERT OR IGNORE INTO PriceBands (StartHour, EndHour, PricePerHour) VALUES
                (0, 7, 500), (7, 14, 1000), (14, 19, 1500), (19, 21, 1000), (21, 24, 500);
            """;
        command.ExecuteNonQuery();
    }
}
