using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.ViewModels
{
    public class AccountModels
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class Response
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }

    public class UserProfile
    {
        [Required]
        public string id { get; set; }

        public string newUsername { get; set; }
        public string newEmail { get; set; }
        public string newLastName { get; set; }
        
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }

    }

    public class UserDto
    {
        public string name { set; get; }
        public string Email { set; get; }
        public string lastName { set; get; }
        public string Address { set; get; }
       
    }
}
