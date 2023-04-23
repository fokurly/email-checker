using Npgsql;

namespace email_check_web.Service.Controllers;

using Microsoft.EntityFrameworkCore;


public class PostgresConnector
{
    private readonly string _connectionString;

    public PostgresConnector(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Domain> CheckDomain(string currentDomain)
    {
        using var dbContext = new MyDbContext(_connectionString);
        return dbContext.ActiveDomain.Where(x => x.ActiveDomain == currentDomain).ToList();
    }
    
    public List<Smtp> CheckSMTP(string currentDomain)
    {
        using var dbContext = new MyDbContext(_connectionString);
        return dbContext.ActiveSMTP.Where(x => x.ActiveSMTP == currentDomain).ToList();
    }
    
    public void InsertDomain(Domain domain)
    {
        using var dbContext = new MyDbContext(_connectionString);
        dbContext.ActiveDomain.Add(domain);
        dbContext.SaveChanges();
    }
    
    public void InsertSMTP(Smtp domain)
    {
        using var dbContext = new MyDbContext(_connectionString);
        dbContext.ActiveSMTP.Add(domain);
        dbContext.SaveChanges();
    }
}

public class MyDbContext : DbContext
{
    public MyDbContext(string connectionString)
        : base(GetOptions(connectionString))
    {
    }

    public DbSet<Domain> ActiveDomain { get; set; }
    public DbSet<Smtp> ActiveSMTP { get; set; }

    private static DbContextOptions GetOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return optionsBuilder.Options;
    }
}

public class Domain
{
    public string ActiveDomain { get; set; }
}

public class Smtp
{
    public string ActiveSMTP { get; set; }
}