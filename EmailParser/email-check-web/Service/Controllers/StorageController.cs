namespace email_check_web.Service.Controllers;

using Microsoft.EntityFrameworkCore;


public class MyDbContext : DbContext
{
    public MyDbContext()
     
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
    public DbSet<Domain> ActiveDomains { get; set; }
    public DbSet<Smtp> ActiveSMTPs { get; set; }
}

public class Domain
{
    public int Id { get; set; }
    public string ActiveDomain { get; set; }
}

public class Smtp
{
    public int Id { get; set; }
    public string ActiveSMTP { get; set; }
}