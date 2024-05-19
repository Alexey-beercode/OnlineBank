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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var workFactor = 12;
        var salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("1025556478955466Admin445", salt);
        modelBuilder.Entity<Role>().HasData(new Role
        {
            Id = new Guid("44546e06-8719-4ad8-b88a-f271ae9d6eab"),
            Name = "Admin",
            Level = 5
        });
        modelBuilder.Entity<Role>().HasData(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Resident",
            Level = 1
        });
        modelBuilder.Entity<Role>().HasData(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Resident",
            Level = 3
        });
        modelBuilder.Entity<TransactionType>().HasData(new TransactionType()
        {
            Id = Guid.NewGuid(),
            Name = "Снятие",
        });
        modelBuilder.Entity<TransactionType>().HasData(new TransactionType()
        {
            Id = Guid.NewGuid(),
            Name = "Пополнение",
        });
        modelBuilder.Entity<TransactionType>().HasData(new DepositType()
        {
            Id = Guid.NewGuid(),
            Name = "Фиксированной процентной ставкой ",
        });
        modelBuilder.Entity<TransactionType>().HasData(new DepositType()
        {
            Id = Guid.NewGuid(),
            Name = "С возможностью пополнения",
        });

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.NewGuid(),
            Login = "Admin",
            PasswordHash = hashedPassword,
            IsDeleted = false
        });
    }

}

