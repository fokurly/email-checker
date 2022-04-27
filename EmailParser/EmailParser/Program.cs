using System.Collections;
using System.Data;
using ExcelDataReader;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using DnsClient;
using EmailParser;
using EmailParser.Resources;

await ReadInfo();

/*string path = @"C:\Users\buval\RiderProjects\EmailParser\EmailParser\Resources\Emails2Check.xlsx";
XlsxParser parser = new XlsxParser();
List<Email> emailList = parser.GetEmailList(path);
EmailChecker checker = new EmailChecker(emailList);
checker.StartCheck();*/

DomainHelper dm = new DomainHelper();

foreach (var domain in DomainHelper.GetAvailableDomain())
{
    dm.MxRecordsExists(domain);
}

/*foreach (var smtp in DomainHelper.GetSmtpDomain())
{
    WriteSmtpDomain.Write(smtp.Key + " " + smtp.Value);
}
Console.WriteLine(dm.MxRecordsExists("google.ru"));*/

// Читаем известные данные.
async Task ReadInfo()
{
    using (StreamReader streamReader = new StreamReader("avaiable_domen.txt"))
    {
        string? line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            DomainHelper.GetAvailableDomain().Add(line);
        }
    }

    using (StreamReader streamReader = new StreamReader("unavalable_domen.txt"))
    {
        string? line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            DomainHelper.GetUnavailableDomain().Add(line);
        }
    }
}
