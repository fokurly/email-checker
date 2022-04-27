using EmailParser.Enums;

namespace EmailParser.Resources;

public class Email
{
    private string _mxServer;
    public EmailStatus Status;
    public Email(string name)
    {
        Name = name;
        if (name.Split("@").Length != 2)
        {
            Domain = "";
            Status = EmailStatus.Fictional;
        }
        else
        {
            Domain = name.Split("@")[1];
            Status = EmailStatus.Unknown;
        }
    }
    
    public string Name { get; }

    public string Domain { get; }
    
    public override bool Equals(object obj)
    {
        var item = obj as Email;

        if (item == null)
        {
            return false;
        }

        return this.Name.Equals(item.Name);
    }
    
    
    public override int GetHashCode()
    {
        return this.Name.GetHashCode();
    }
}