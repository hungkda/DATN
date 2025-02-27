using System.ComponentModel.DataAnnotations;

namespace DATN.ViewModels
{
    public class Email
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}