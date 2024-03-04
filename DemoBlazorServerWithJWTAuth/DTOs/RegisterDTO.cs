using System.ComponentModel.DataAnnotations;

namespace DemoBlazorServerWithJWTAuth.DTOs
{
    public class RegisterDTO : LoginDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        //[Required, Compare(nameof(Password)), DataType(DataType.Password)]
        //public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;


    }
}
