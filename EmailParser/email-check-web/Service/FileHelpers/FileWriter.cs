namespace email_check_web.Service.FileHelpers;

public static class FileWriter
{
    /// <summary>
    /// Writes result of checking in a file.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="fileName"></param>
    public static void WriteResultMaleToFile(Email email, string fileName)
    {
        fileName += ".txt";
        using StreamWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(email.Name + "; " + email.PassedChecks);
    }
    
    
    /// <summary>
    /// Writes information about smtp servers for file.
    /// </summary>
    /// <param name="str"></param>
    public static void WriteSmtpServerWithDomainToFile(string str)
    {
        using StreamWriter writer = new StreamWriter("available_smtp_domain.txt", true);
        writer.WriteLine(str);
    }
}