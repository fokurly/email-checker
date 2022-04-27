using System.Collections;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text.RegularExpressions;
using EmailParser.Enums;
using EmailParser.Resources;

namespace EmailParser;

public class EmailChecker
{
    private List<Email> _emails;
    private string MxServer;
    
    public EmailChecker(List<Email> emails)
    {
        _emails = emails;
    }

    public void StartCheck()
    {
        // Проверка на формат.
        for (int i = 0; i < _emails.Count; ++i)
        {
            if (_emails[i].Status == EmailStatus.Unknown)
                if (!IsValidStringEmail(_emails[i].Name))
                {
                    _emails[i].Status = EmailStatus.Fictional;
                }
        }
        
        Console.WriteLine("Format checking done");
        
        // Проверка доступности домена.
        DomainHelper helper = new DomainHelper();
        for (int i = 0; i < _emails.Count; ++i)
        {
            string domain = _emails[i].Domain;
            if (_emails[i].Status == EmailStatus.Unknown)
                if (!helper.IsExists(domain))
                {
                    _emails[i].Status = EmailStatus.Fictional;
                }
        }
        
        Console.WriteLine("domain checking done.");
        // Проверка на mx записи почты
        WriteResultInFiles();
    }

    private void WriteResultInFiles()
    {
        Console.WriteLine("Запись в файлы....");
        for (int i = 0; i < _emails.Count; ++i)
        {
            if (_emails[i].Status == EmailStatus.Fictional)
            {
                if (_emails[i].Status == EmailStatus.Fictional)
                    FileWriter.WriteFictionalMaleToFile(_emails[i].Name);
            }
        }

        foreach (var domain in DomainHelper.GetAvailableDomain())
        {
            WriteAvailableDomain.Write(domain);
        }
        
        foreach (var domain in DomainHelper.GetUnavailableDomain())
        {
            WriteUnavalableDomain.Write(domain);
        }
    }
    
    /// <summary>
    /// Tries to create email with C# class MailAdress.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public bool IsValidStringEmail(string email)
    {
        return MailAddress.TryCreate(email, out var mailAddress);
    }
}