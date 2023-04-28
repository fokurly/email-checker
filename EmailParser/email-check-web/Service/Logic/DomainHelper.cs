using System.Net;
using System.Net.Sockets;
using System.Text;
using DnsClient;
using DnsClient.Protocol;
using email_check_web.Service.FileHelpers;
using Microsoft.Store.PartnerCenter;

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
            if (_unavailableHost.Contains(domain))
            {
                return false;
            }

            if (smtpServer.ContainsKey(domain))
            {
                return true;
            }


            IPHostEntry hostEntry = await Dns.GetHostEntryAsync(domain);
            bool res = await TryToAddSmtpServerToDomain(domain);
            if (!res && _availableHost.Contains(domain))
            {
                res = await TryToAddSmtpServerToDomain(domain);
                if (!res && _availableHost.Contains(domain))
                {
                    return await TryToAddSmtpServerToDomain(domain);
                }
            }

            return res;
        }
        catch (Exception e)
        {
            _unavailableHost.Add(domain);
            return false;
        }
    }
    
    /// <summary>
    /// Tries to find smtp server on current domain.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    private async Task<bool> TryToAddSmtpServerToDomain(string domain)
    {
        try
        {
            var lookup = new LookupClient();
            var result = lookup.QueryAsync(domain, QueryType.MX);
            await result;

            if (result.Result.Answers.Count == 0)
                return false;

            DnsResourceRecord record = result.Result.Answers[0];
            string[] stringSeparators = {" mx ", " MX ", " Mx "};

            if (!record.ToString().Contains("mx") && !record.ToString().Contains("Mx") &&
                !record.ToString().Contains("MX"))
            {
                return false;
            }

            string smtp = record.ToString().Split(stringSeparators, StringSplitOptions.None)[1].Split(" ")[1];
            smtp = smtp.Remove(smtp.Length - 1);
            string domainName = record.DomainName.ToString().Remove(record.DomainName.ToString().Length - 1);
            smtpServer.Add(domainName, smtp);

            return true;
        }
        catch (DnsClient.DnsResponseException)
        {
            _longResponse.Add(domain);
            return false;
        }
    }

    /// <summary>
    /// Method tries to find mx records on smtp server of current email. 
    /// </summary>
    /// <param name="email"></param>
    public async Task StartSmtpCheck(Email email)
    {
        TcpClient newClient = new TcpClient();
        try
        {
            // Mail.ru always returns true, yahoo.com just blocks your ip address. Unreal to check emails on this domains with current way.
            if (email.Domain is "yahoo.com" or "mail.ru")
            {
                email.Status = EmailStatus.Real;
                return;
            }

            string smtpp = GetSmtpDomain()[email.Domain];
            await newClient.ConnectAsync(smtpp, 25);

            const string CRLF = "\r\n";
            var data = "EHLO lo" + CRLF;
            byte[] szData = Encoding.ASCII.GetBytes(data.ToCharArray());
            NetworkStream netStrm = newClient.GetStream();
            netStrm.ReadTimeout = 7000;
            newClient.Client.ReceiveTimeout = 7000;
            netStrm.Write(szData, 0, szData.Length);

            ReadDataFromNetStream(ref netStrm);
            ReadDataFromNetStream(ref netStrm);

            data = string.Format("MAIL FROM: <buva.art@ya.ru>\n");
            szData = System.Text.Encoding.ASCII.GetBytes(data.ToCharArray());
            netStrm.Write(szData, 0, szData.Length);

            ReadDataFromNetStream(ref netStrm);

            string rcpt = email.Name;
            data = "RCPT TO: <" + rcpt + ">\n";
            szData = System.Text.Encoding.ASCII.GetBytes(data.ToCharArray());
            netStrm.Write(szData, 0, szData.Length);

            email.Status = ParseSmtpServerResponse(ReadDataFromNetStream(ref netStrm))
                ? EmailStatus.Real
                : EmailStatus.Fictional;

            netStrm.Close();
        }
        catch (Exception)
        {
            email.Status = EmailStatus.Fictional;
        }
        finally
        {
            newClient.Close();
        }
    }


    /// <summary>
    /// Reads data from NetworkStream.
    /// </summary>
    /// <param name="netStrm"></param>
    /// <returns></returns>
    private string ReadDataFromNetStream(ref NetworkStream netStrm)
    {
        StringBuilder response = new StringBuilder();
        byte[] data = new byte[1024];
        do
        {
            int bytes = netStrm.Read(data, 0, data.Length);
            response.Append(Encoding.UTF8.GetString(data, 0, bytes));
        } while (netStrm.DataAvailable);

        return response.ToString();
    }

    
    /// <summary>
    /// If response from server contains 250 - ok. Any other - false;
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private bool ParseSmtpServerResponse(string response)
    {
        if (response.Contains("250"))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Getter of _availableHost.
    /// </summary>
    /// <returns></returns>
    public static HashSet<string> GetAvailableDomain()
    {
        return _availableHost;
    }

    /// <summary>
    /// Getter of _unavalableHost.
    /// </summary>
    /// <returns></returns>
    public static HashSet<string> GetUnavailableDomain()
    {
        return _unavailableHost;
    }


    /// <summary>
    /// Getter of smtpServer.
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, string> GetSmtpDomain()
    {
        return smtpServer;
    }

    /// <summary>
    /// Getter for _longResponse.
    /// </summary>
    /// <returns></returns>
    public HashSet<string> GetLongResponse()
    {
        return _longResponse;
    }
}