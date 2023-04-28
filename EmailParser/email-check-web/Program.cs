using email_check_web.Service.FileHelpers;
using EmailParser;

await FileReader.ReadInfo();
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseStaticFiles();

app.Run(async (context) =>
{
    var response = context.Response;
    context.Response.ContentType = "text/html; charset=utf-8";

    if (context.Request.Path == "/sendEmail")
    {
        string id = Guid.NewGuid().ToString();

        var form = context.Request.Form;
        string email = form["email"];
        
        {
            EmailChecker checker = new EmailChecker(email, id);
            await checker.StartCheck();
            checker.WriteResultInFiles();
            context.Response.Headers.ContentDisposition = "attachment; filename=result.txt";
            await context.Response.SendFileAsync("result_" + id + ".txt");
            File.Delete("result_" + id + ".txt");
        }
    }
    else if (context.Request.Method == "POST" && context.Request.Path == "/upload")
    {
        IFormFileCollection files = context.Request.Form.Files;
        if (files.Count == 0)
        {
            await context.Response.SendFileAsync("wwwroot/index.html");
        }

        var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
        Directory.CreateDirectory(uploadPath);
        foreach (var file in files)
        {
            if (Path.GetExtension(file.FileName) != ".xlsx")
            {
                await context.Response.SendFileAsync("wwwroot/index.html");
            }
            else
            {
                string id = Guid.NewGuid().ToString();
                string fullPath = $"{uploadPath}/{file.FileName}";

                await using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                XlsxParser parser = new XlsxParser();
                EmailChecker checker = new EmailChecker(parser.GetEmailList(fullPath), id);
                await checker.StartCheck();
                checker.WriteResultInFiles();
                context.Response.Headers.ContentDisposition = "attachment; filename=result.txt";
                await context.Response.SendFileAsync("result_" + id + ".txt");
                File.Delete("result_" + id + ".txt");
                File.Delete(fullPath);
            }
        }
    }
    else
    {
        await context.Response.SendFileAsync("wwwroot/index.html");
    }
});

app.Run();