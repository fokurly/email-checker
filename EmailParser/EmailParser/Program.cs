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

//await ReadInfo();


string path = @"C:\Users\buval\RiderProjects\EmailParser\EmailParser\Resources\Emails2Check.xlsx";
XlsxParser parser = new XlsxParser();
EmailChecker checker = new EmailChecker(parser.GetEmailList(path));
await checker.StartCheck();
checker.WriteResultInFiles();

/*Email email = new Email("buvaltsev.art@yandex.ru");
HashSet<Email> set = new HashSet<Email> {email};

EmailChecker check = new EmailChecker(set);
await check.StartCheck();*/

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
