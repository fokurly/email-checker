using System.Collections;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using EmailParser.Enums;
using EmailParser.Resources;

namespace EmailParser;

public class EmailChecker
{
    private HashSet<Email> _emails;

    public EmailChecker(HashSet<Email> emails)
    {
        _emails = emails;
    }

    public async Task StartCheck()
    {
        Console.WriteLine("Number of email for checking - " + _emails.Count);
        
        Console.WriteLine("Start format checking.....");
        int fictionalCount = 0;
        foreach (Email mail in _emails)
        {
            if (mail.Status == EmailStatus.Unknown)
                if (!IsValidStringEmail(mail.Name))
                {
                    mail.Status = EmailStatus.Fictional;
                    fictionalCount++;
                }
        }

        Console.WriteLine("Format checking done");
        Console.WriteLine("Number of fictional email - " + fictionalCount);
        // Проверка доступности домена.
        Console.WriteLine("Start checking availability of domain....");

        HashSet<string> tempDomains = new HashSet<string>();
        foreach (var email in _emails)
        {
            if (email.Status != EmailStatus.Fictional)
                tempDomains.Add(email.Domain);
        }

        DomainHelper helper = new DomainHelper();
        var domains = from s in tempDomains.AsParallel()
            select helper.IsExists(s);

        await Task.WhenAll();

        foreach (var result in domains)
        {
            await result;
        }

        foreach (Email email in _emails)
        {
            if (!DomainHelper.GetSmtpDomain().ContainsKey(email.Domain))
            {
                email.Status = EmailStatus.Fictional;
                ++fictionalCount;
            }
        }

        Console.WriteLine("Availability and existing of mx records completed.");
        Console.WriteLine("Number of fictional email - " + fictionalCount);

        Console.WriteLine("Checking for mx records of current email.....");

        var emailss = from s in _emails.AsParallel()
            where s.Status != EmailStatus.Fictional
            select StartSmtpCheck(s);

        await Task.WhenAll();
        foreach (var res in emailss)
        {
            await res;
        }

        Console.WriteLine("Checking done.");
        Console.WriteLine("Number of fictional email - " + fictionalCount);
    }

    private async Task StartSmtpCheck(Email email)
    {
        try
        {
            TcpClient newClient = new TcpClient();
            string smtpp = DomainHelper.GetSmtpDomain()[email.Domain];
            Console.WriteLine(smtpp);
            newClient.Connect(smtpp, 25);

            string Data;
            byte[] szData;
            string CRLF = "\r\n";
            Data = "EHLO lo" + CRLF;
            szData = Encoding.ASCII.GetBytes(Data.ToCharArray());
            NetworkStream netStrm = newClient.GetStream();
            netStrm.ReadTimeout = 7000;
            newClient.Client.ReceiveTimeout = 7000;
            StreamReader RdStrm = new StreamReader(newClient.GetStream());
            netStrm.Write(szData, 0, szData.Length);

            ReadDataFromNetStream(ref netStrm, false);
            ReadDataFromNetStream(ref netStrm, false);

            Data = string.Format("MAIL FROM: <buva.art@ya.ru>\n");
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            netStrm.Write(szData, 0, szData.Length);

            ReadDataFromNetStream(ref netStrm, false);

            string rcpt = email.Name;
            Data = "RCPT TO: <" + rcpt + ">\n";
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            netStrm.Write(szData, 0, szData.Length);

            ReadDataFromNetStream(ref netStrm, true);
        }
        catch (Exception)
        {
            // Добавить нормальное исключение на случай окончания срока жизни 
        }
    }

    private void ReadDataFromNetStream(ref NetworkStream netStrm, bool flag)
    {
        StringBuilder response = new StringBuilder();
        byte[] data = new byte[1024];
        do
        {
            int bytes = netStrm.Read(data, 0, data.Length);
            response.Append(Encoding.UTF8.GetString(data, 0, bytes));
        } while (netStrm.DataAvailable);

        if (flag)
            Console.WriteLine(response);
    }

    public void WriteResultInFiles()
    {
        Console.WriteLine("Запись в файлы....");
        foreach (Email mail in _emails)
        {
            if (mail.Status == EmailStatus.Fictional)
                FileWriter.WriteFictionalMaleToFile(mail.Name);
            else
                FileWriter.WriteRealMaleToFile(mail.Name);
        }

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
}