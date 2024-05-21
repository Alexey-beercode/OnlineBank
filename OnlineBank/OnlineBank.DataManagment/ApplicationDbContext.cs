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
}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var workFactor = 12;
        var salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("1025556478955466Admin445", salt);
        modelBuilder.Entity<Role>().HasData(new Role
        {
            Id = new Guid("36111A6D-9151-46FA-A954-7B37160D52DB"),
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
            Name = "Manager",
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
        modelBuilder.Entity<DepositType>().HasData(new DepositType()
        {
            Id = Guid.NewGuid(),
            Name = "С Фиксированной процентной ставкой ",
            InterestRate = 5
        });
        modelBuilder.Entity<DepositType>().HasData(new DepositType()
        {
            Id = Guid.NewGuid(),
            Name = "С возможностью пополнения",
            InterestRate = new decimal(3.5)
        });

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = new Guid("36111A6D-9151-46FA-A954-7B37160D53DB"),
            Login = "Admin",
            PasswordHash = hashedPassword,
            IsDeleted = false
        });
        modelBuilder.Entity<UserRole>().HasData(new UserRole()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            RoleId = new Guid("36111A6D-9151-46FA-A954-7B37160D52DB"),
            Userid = new Guid("36111A6D-9151-46FA-A954-7B37160D53DB")
        });
    }

}

