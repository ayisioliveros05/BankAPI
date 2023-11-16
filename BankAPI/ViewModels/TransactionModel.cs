using BankAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace BankAPI.ViewModels
{
    public class TransactionModel
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
