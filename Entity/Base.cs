using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Base
    {
        public string CreatedBy { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
