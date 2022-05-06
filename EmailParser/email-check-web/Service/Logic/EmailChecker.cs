using System.Collections;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using email_check_web.Service.FileHelpers;

namespace EmailParser;

public class EmailChecker
{
    private HashSet<Email> _emails;
    private HashSet<Email> _lastListEmail;
    private string _id;

    public EmailChecker(HashSet<Email> emails, string id = "")
    {
        _emails = emails;
        _lastListEmail = new HashSet<Email>();
        _id = id;
    }

    public EmailChecker(string email, string id = "")
    {
        _emails = new HashSet<Email>();
        _emails.Add(new Email(email));
        _lastListEmail = new HashSet<Email>();
        _id = id;
    }

    public async Task StartCheck()
    {
        int fictionalCount = 0;
        foreach (Email mail in _emails)
        {
            if (mail.Status == EmailStatus.Unknown)
                if (!IsValidStringEmail(mail.Name))
                {
                    mail.Status = EmailStatus.Fictional;
                    fictionalCount++;
                    mail.PassedChecks = "fictional";
                }
                else
                {
                    mail.PassedChecks += "format ok; ";
                }
        }

        HashSet<string> tempDomains = new HashSet<string>(from obj in _emails
            where obj.Status == EmailStatus.Unknown
            select obj.Domain);

        DomainHelper helper = new DomainHelper();
        var domains = tempDomains.AsParallel().Select(s => helper.IsExists(s));
        await Task.WhenAll(domains);

        foreach (Email email in _emails)
        {
            if (email.Status == EmailStatus.Unknown)
                if (!DomainHelper.GetSmtpDomain().ContainsKey(email.Domain))
                {
                    email.Status = EmailStatus.Fictional;
                    email.PassedChecks += "Domain is not responding; ";
                    ++fictionalCount;
                }
                else
                {
                    email.PassedChecks += "Domain is ok; ";
                }
        }
        
        foreach (var s in _emails)
        {
            if (s.Status == EmailStatus.Unknown)
            {
                _lastListEmail.Add(s);
            }
        }

        var emailss = _lastListEmail.AsParallel().Select(s => helper.StartSmtpCheck(s));
        await Task.WhenAll(emailss);


        foreach (Email email in _lastListEmail)
        {
            if (email.Status == EmailStatus.Fictional)
            {
                email.PassedChecks += "Mx records does not exists";
                ++fictionalCount;
            }
            else
            {
                email.PassedChecks += "Mx records is ok. All checks passed";
            }
        }

        Console.WriteLine("Checking done.");
        Console.WriteLine("Number of fictional email - " + fictionalCount);

        fictionalCount = 0;
    }


    public void WriteResultInFiles()
    {
        Console.WriteLine("Запись в файлы....");
        foreach (Email mail in _emails)
        {
            FileWriter.WriteResultMaleToFile(mail, "result_" + _id);
        }

        //  В конце дописать статистику.
        foreach (var smtp in DomainHelper.GetSmtpDomain())
        {
            FileWriter.WriteSmtpServerWithDomenToFile(smtp.Key + " " + smtp.Value);
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

    public Email GetEmail()
    {
        return _emails.First();
    }
}