using System.ComponentModel.DataAnnotations;

namespace mvcCookieAuthSample.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName{get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password{get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmedPassword{get;set;}

    }   
}
