using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;

namespace OnlineBank.DataManagment;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UsersRoles { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<DepositType> DepositTypes { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }

public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
