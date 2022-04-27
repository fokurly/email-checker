using EmailParser.Resources;

namespace EmailParser;

public interface IXlsxParser
{
    HashSet<Email> GetEmailList(string path);
}