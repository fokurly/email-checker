namespace EmailParser;

public class WriteSmtpDomain
{
    public static string path = "smtpDomain.txt";
    
    public static void Write(string str)
    {
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(str);
        }
    }
}