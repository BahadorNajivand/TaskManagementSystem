using ModelsAndEnums.Enums;
using System.ComponentModel.DataAnnotations;

namespace ModelsAndEnums.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string PasswordSalt { get; set; }


        public UserRole Role { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public void SetPassword(string password)
        {
            PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            Password = BCrypt.Net.BCrypt.HashPassword(password, PasswordSalt);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }
    }
}
