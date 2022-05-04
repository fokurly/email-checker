namespace email_check_web.Service.FileHelpers;

public static class FileWriter
{
    public static void WriteResultMaleToFile(Email email, string fileName)
    {
        fileName += ".txt";
        using StreamWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(email.Name + " " + email.PassedChecks);
    }

    public static void WriteSmtpServerWithDomenToFile(string str)
    {
        using StreamWriter writer = new StreamWriter("available_smtp_domain.txt", true);
        writer.WriteLine(str);
    }
}