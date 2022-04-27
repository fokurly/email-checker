namespace EmailParser;

public static class FileWriter
{
    public static string path = "ficitonal_email";
    
    public static void WriteFictionalMaleToFile(string str)
    {
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(str);
        }
    }
}