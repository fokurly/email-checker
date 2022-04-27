using System.Net;
using System.Runtime.CompilerServices;
using DnsClient;
using DnsClient.Protocol;
using EmailParser.Resources;

namespace EmailParser;

public class DomainHelper
{
    private static HashSet<string> _availableHost = new HashSet<string>();
    private static Dictionary<string, string> smtpServer = new Dictionary<string, string>();
    private static HashSet<string> _unavailableHost = new HashSet<string>();
    private HashSet<string> _longResponse;

    public DomainHelper()
    {
        _longResponse = new HashSet<string>();
    }

    /// <summary>
    /// Method tries to connect to the current domain.
    /// </summary>
    /// <param name="domain">Domain name</param>
    /// <returns>true if connection successful, else false</returns>
    public async Task<bool> IsExists(string domain)
    {
        try
        {
            //Console.WriteLine("Пробуем подключиться");
            if (_unavailableHost.Contains(domain))
            {
                return false;
            }

            if (smtpServer.ContainsKey(domain))
            {
                return true;
            }

            IPHostEntry hostEntry = Dns.GetHostEntry(domain);
            return await TryToAddSmtpServerToDomain(domain);
        }
        catch (Exception e)
        {
          //  Console.WriteLine("Ошибка подключения");
            _unavailableHost.Add(domain);
            return false;
        }
    }

    private async Task<bool> TryToAddSmtpServerToDomain(string domain)
    {
        try
        {
            //Console.WriteLine("check started");
            var lookup = new LookupClient();
            var result = lookup.QueryAsync(domain, QueryType.MX);
            await result;

            if (result.Result.Answers.Count == 0)
                return false;

            // Переписать под цикл, который перебирает все полученные записи, если в первой не попалось mx.
            DnsResourceRecord record = result.Result.Answers[0];
            string[] stringSeparators = new string[] {" mx ", " MX ", " Mx "};

            // Переделать под дургую проверку. Почему sochi.mail.ru ссылается на mx.yandex.ru?
            if (!record.ToString().Contains("mx") && !record.ToString().Contains("Mx") &&
                !record.ToString().Contains("MX"))
                return false;

            string smtp = record.ToString().Split(stringSeparators, StringSplitOptions.None)[1].Split(" ")[1];
            smtp = smtp.Remove(smtp.Length - 1);
            string domainName = record.DomainName.ToString().Remove(record.DomainName.ToString().Length - 1);
            smtpServer.Add(domainName, smtp);
            return true;
        }
        catch (DnsClient.DnsResponseException)
        {
            // Плохие респонсы записать в отдельный список и перепроверить.
            // Написать логику под перепровекру долгих респонсов и удаления из недоступных в случае успеха.
            _longResponse.Add(domain);
            return false;
        }
    }
    

    public static HashSet<string> GetAvailableDomain()
    {
        return _availableHost;
    }

    public static HashSet<string> GetUnavailableDomain()
    {
        return _unavailableHost;
    }

    public static Dictionary<string, string> GetSmtpDomain()
    {
        return smtpServer;
    }

    public HashSet<string> GetLongResponse()
    {
        return _longResponse;
    }
}