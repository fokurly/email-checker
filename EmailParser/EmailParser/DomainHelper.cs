using System.Net;
using System.Runtime.CompilerServices;
using DnsClient;
using DnsClient.Protocol;

namespace EmailParser;

public class DomainHelper
{
    private static HashSet<string> _availableHost = new HashSet<string>();
    private static Dictionary<string, string> smtpServer = new Dictionary<string, string>();
    private static HashSet<string> _unavailableHost = new HashSet<string>();

    public DomainHelper()
    {
    }

    /// <summary>
    /// Method tries to connect to the current domain.
    /// </summary>
    /// <param name="domain">Domain name</param>
    /// <returns>true if connection successful, else false</returns>
    public bool IsExists(string domain)
    {
        try
        {
            if (_availableHost.Contains(domain))
            {
                return true;
            }

            if (_unavailableHost.Contains(domain))
            {
                return false;
            }

            IPHostEntry hostEntry = Dns.GetHostEntry(domain);
            _availableHost.Add(hostEntry.HostName);
            return true;
        }
        catch (Exception e)
        {
            _unavailableHost.Add(domain);
            return false;
        }
    }

    /// <summary>
    /// Checks if server has mx records
    /// </summary>
    /// <param name="adress"></param>
    /// <returns>true if mx records exists, else false</returns>
    public bool MxRecordsExists(string adress)
    {
        try
        {
            var lookup = new LookupClient();
            var result = lookup.Query(adress, QueryType.MX);
            //Console.WriteLine("Started method");
            foreach (var record in result.Answers)
            {
                string[] stringSeparators = new string[] { " mx ", " MX ", " Mx " };
                string smtp = record.ToString().Split(stringSeparators, StringSplitOptions.None)[1].Split(" ")[1];
                if (string.IsNullOrEmpty(smtp))
                {
                    Console.WriteLine(record);
                }

                /*  Console.WriteLine(smtp);
                Console.WriteLine(record.DomainName);
                Console.WriteLine(record);
   */
                // Добавить проверки на null.
                if (!smtpServer.ContainsKey(record.DomainName))
                {
                    smtpServer.Add(record.DomainName, smtp);
                    WriteSmtpDomain.Write(record.DomainName.ToString() + " " + smtp);
                }
            }

            return result.Answers.Count != 0;
        }
        catch (DnsClient.DnsResponseException)
        {
            // Плохие респонсы записать в отдельный список и перепроверить.
            Console.WriteLine(smtpServer);
            return false;
        }
    }

    public void GetMxRecords(DnsQueryResponse list)
    {
        if (list.Answers.Count == 0)
            throw new ArgumentException("Не найдены mx записи.");

        foreach (var record in list.Answers)
        {
            Console.WriteLine(record);
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
}