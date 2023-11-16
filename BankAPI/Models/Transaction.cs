using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAPI.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionUniqueReference { get; set; }
        public decimal TransactionAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }
        public bool isSuccessful => TransactionStatus.Equals(TranStatus.Success); // depends on TranStatus
        public string TransactionSourceAccount { get; set; }
        public string? TransactionDestinationAccount { get; set; } 
        public TranType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

        public Transaction()
        {
            TransactionUniqueReference = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";
        }
    }

    public enum TranStatus
    {
        Success,
        Failed,
        Error
    }

    public enum TranType
    {
        Deposit,
        Withdrawal,
        Transfer,
        BalanceInquiry
    }
}
