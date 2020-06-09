using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vectio.eventmanagement.api.models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string[] Roles { get; set; }
        public string Role { get; set; }
        public string ConfirmationUrl { get; set; }
    }

    public class RegisterUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ConfirmationUrl { get; set; }
    }
    public class UpdateUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string[] Roles { get; set; }
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
