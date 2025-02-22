using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Management
{

    public class Contact
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chủ đề.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
        public string Message { get; set; }
    }

}
