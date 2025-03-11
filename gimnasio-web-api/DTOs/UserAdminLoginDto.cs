using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gimnasio_web_api.DTOs
{
    public class UserAdminLoginDto
    {
        public required int Codigo { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}