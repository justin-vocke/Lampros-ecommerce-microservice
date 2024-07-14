namespace Lampros.MVC.Models.Dto
{
    public record RegistrationRequestDto(string Email, string FirstName, string LastName, string PhoneNumber, string Password, string? RoleName);
}
