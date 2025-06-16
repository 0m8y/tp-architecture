using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
}
