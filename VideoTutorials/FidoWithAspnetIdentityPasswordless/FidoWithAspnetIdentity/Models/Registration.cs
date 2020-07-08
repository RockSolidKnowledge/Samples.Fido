using System.ComponentModel.DataAnnotations;

namespace FidoWithAspnetIdentity.Models
{
    public class Registration
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }    }
}