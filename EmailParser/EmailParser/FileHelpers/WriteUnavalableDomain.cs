namespace EmailParser;

public class WriteUnavalableDomain
{
    public static string path = "unavalable_domen.txt";
    
    public static void Write(string str)
    {
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(str);
        }
    }
}