﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bookify.DTO
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public BookStatus Status { get; set; }
    }
    public enum BookStatus {
        Borrowed,
        Returned
    }
}
