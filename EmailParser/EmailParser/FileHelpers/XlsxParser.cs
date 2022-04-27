using System.Data;
using EmailParser.Resources;
using ExcelDataReader;

namespace EmailParser;

public class XlsxParser : IXlsxParser
{
    public List<Email> GetEmailList(string path)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        FileStream fStream = File.Open(path,
            FileMode.Open, FileAccess.Read);
        IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
        DataSet resultDataSet = excelDataReader.AsDataSet();

        List<Email> emails = new List<Email>(resultDataSet.Tables[0].Rows.Count);
        Console.WriteLine(emails.Capacity);
        for (var i = 1; i < resultDataSet.Tables[0].Rows.Count; i++)
        {
            for (var j = 0; j < resultDataSet.Tables[0].Columns.Count; j++)
            {
                // Написать для уникальных 
                if (!String.IsNullOrEmpty(resultDataSet.Tables[0].Rows[i][j].ToString()))
                    emails.Add(new Email(resultDataSet.Tables[0].Rows[i][j].ToString()));
            }
        }

        // Console.WriteLine(emails[^1]);
        excelDataReader.Close();
        fStream.Close();

        Console.WriteLine("Получили список");
        return emails;
    }
}