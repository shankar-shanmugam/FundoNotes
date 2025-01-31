using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class ForgetPasswordModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }
    }
}
