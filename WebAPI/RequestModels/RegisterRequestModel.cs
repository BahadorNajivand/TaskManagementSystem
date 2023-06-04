using System.ComponentModel.DataAnnotations;

namespace WebAPI.RequestModels
{
    public class RegisterRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
