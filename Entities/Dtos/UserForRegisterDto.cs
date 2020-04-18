using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Core.Entities;

namespace Entities.Dtos
{
    public class UserForRegisterDto : IDto
    {
        [Required(ErrorMessage ="Email boş geçilemez")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre boş geçilemez")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Şifre boş geçilemez")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "İsim boş geçilemez")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Soyisim boş geçilemez")]
        public string LastName { get; set; }
    }
}
