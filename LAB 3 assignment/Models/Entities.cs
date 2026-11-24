namespace LAB_3_assignment.Models;

public enum UserRole { Administrator, Attendant }
public enum BicycleStatus { Available, Rented, Maintenance }
public enum PaymentMethod { Cash, Card, MobileMoney }

public sealed record User(int Id, string Username, string Password, UserRole Role);
public sealed record Customer(int Id, string Name, string Phone, string IdentificationNumber);
public sealed record Bicycle(int Id, string Code, string Type, BicycleStatus Status, int RentalCount);
public sealed record Rental(int Id, int CustomerId, string CustomerName, int BicycleId, string BicycleCode, int StartHour, int EndHour, decimal TotalPrice, bool IsPaid, DateTime CreatedAt);
public sealed record Payment(int Id, int RentalId, decimal Amount, DateTime PaymentDate, PaymentMethod Method);
public sealed record PriceBand(int StartHour, int EndHour, decimal PricePerHour);
