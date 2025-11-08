namespace FinalProject.Application.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        public string? University { get; set; }
        public bool? Active { get; set; }
    }
}