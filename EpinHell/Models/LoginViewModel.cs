using System.ComponentModel.DataAnnotations;

namespace EpinHell.Models
{

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
