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
        using StreamReader streamReader = new StreamReader("Service/Resources/avaiable_domain.txt");
        string? line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            DomainHelper.GetAvailableDomain().Add(line);
        }
    }
}