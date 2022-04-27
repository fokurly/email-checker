using EmailParser.Resources;

namespace EmailParser;

public interface IXlsxParser
{
    List<Email> GetEmailList(string path);
}