namespace EmailParser;

public class WriteAvailableDomain
{
    public static string path = "avaiable_domen.txt";
    
    public static void Write(string str)
    {
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(str);
        }
    }
}