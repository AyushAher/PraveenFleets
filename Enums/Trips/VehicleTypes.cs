using System.ComponentModel;

namespace Enums.Trips;

public enum VehicleTypes : byte
{
    [Description("Sedan")] Sedan = 10,
    [Description("Coupe")] Coupe = 20,
    [Description("Hatchback")] Hatchback = 30,
    [Description("Convertible")] Convertible = 40,
    [Description("Sports Car")] SportsCar = 50,
    [Description("SUV")] SUV = 60,
    [Description("Crossover")] Crossover = 70,
    [Description("Electric Car")] ElectricCar = 80,
}