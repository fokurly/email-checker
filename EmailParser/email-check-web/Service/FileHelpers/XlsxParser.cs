using System.Data;
using ExcelDataReader;
namespace email_check_web.Service.FileHelpers;

public class XlsxParser : IXlsxParser
{
    /// <summary>
    /// Method reads .xlsx file from 1 to last line and adds line which is not null or empty for
    /// further verification.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public HashSet<Email> GetEmailList(string path)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        FileStream fStream = File.Open(path,
            FileMode.Open, FileAccess.Read);
        IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
        DataSet resultDataSet = excelDataReader.AsDataSet();

        HashSet<Email> emails = new HashSet<Email>(resultDataSet.Tables[0].Rows.Count);

        for (var i = 0; i < resultDataSet.Tables[0].Rows.Count; i++)
        {
            for (var j = 0; j < resultDataSet.Tables[0].Columns.Count; j++)
            {
                if (!String.IsNullOrEmpty(resultDataSet.Tables[0].Rows[i][j].ToString()))
                    emails.Add(new Email(resultDataSet.Tables[0].Rows[i][j].ToString()));
            }
        }
        
        excelDataReader.Close();
        fStream.Close();
        return emails;
    }
}