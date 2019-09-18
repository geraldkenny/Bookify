using System;
using System.Collections.Generic;
using System.Text;

namespace Bookify.DTO
{
    public class BaseDTO
    {
        public string CreatedBy { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
