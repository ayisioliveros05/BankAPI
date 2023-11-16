using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.DAL
{
    public class BankAPIDbContext : DbContext
    {
        public BankAPIDbContext(DbContextOptions<BankAPIDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
