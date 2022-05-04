namespace email_check_web.Service.FileHelpers;

public static class FileReader
{
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