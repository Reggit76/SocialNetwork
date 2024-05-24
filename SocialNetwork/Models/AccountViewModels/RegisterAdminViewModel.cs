using System.ComponentModel.DataAnnotations;
using SocialNetwork.Models.Entity;

namespace SocialNetwork.Models.AccountViewModels
{
    public class RegisterAdminViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [Display(Name = "Role")]
        public UserRole Role { get; set; }
    }
}
