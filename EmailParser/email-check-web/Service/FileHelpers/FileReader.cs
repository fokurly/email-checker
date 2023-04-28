using email_check_web.Service.Controllers;
//using email_check_web.Service.Controllers;

namespace email_check_web.Service.FileHelpers;

public static class FileReader
{
    /// <summary>
    /// Reads information about available domains.
    /// If you have a file with available domains which where checked in last 24 hours
    /// Checking of current domains will be skipped and goes to smtp checking.
    /// </summary>
    public static async Task ReadInfo() 
    {

        using (MyDbContext db = new MyDbContext())
        {
            var domains = db.ActiveDomains.ToList();
            foreach(Domain d in domains)
            {
                DomainHelper.GetAvailableDomain().Add(d.ActiveDomain);
            }
        }
    }
}