namespace EmailParser;

public static class FileWriter
{
    public static void WriteFictionalMaleToFile(string str)
    {
        using (StreamWriter writer = new StreamWriter("fictional_email.txt", true))
        {
            writer.WriteLine(str);
        }
    }
    
    public static void WriteRealMaleToFile(string str)
    {
        using (StreamWriter writer = new StreamWriter("real_emails.txt", true))
        {
            writer.WriteLine(str);
        }
    }
    
    
    public static void WriteSmtpServerWithDomenToFile(string str)
    {
        using (StreamWriter writer = new StreamWriter("available_smtp_domain.txt", true))
        {
            writer.WriteLine(str);
        }
    }
    
}