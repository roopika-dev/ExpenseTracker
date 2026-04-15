using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.DTOs.Auth
{
    public class UpdateProfileDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
