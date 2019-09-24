using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public BookStatus Status { get; set; }
        public DateTime BorrowedAt { get; set; }
    }
    public enum BookStatus {
        Borrowed,
        Returned
    }
}
