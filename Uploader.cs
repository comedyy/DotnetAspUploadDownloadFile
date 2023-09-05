


using System.Net;
using System.Net.Http.Headers;

namespace PizzaStore;

public class Uploader
{
    static string _lastUploadName = "";
    async internal static Task Download(HttpContext context)
    {
        var name = context.Request.Form["name"];
        context.Response.ContentType = "image/png";
        using (var fileStream = File.OpenRead($"save/{name}")) 
        {
            await fileStream.CopyToAsync(context.Response.Body);
        }
    }

    internal static IResult GetLastetPlayback(HttpRequest request)
    {
        return Results.Ok(_lastUploadName);
    }

    async internal static Task<IResult> Upload(HttpRequest request)
    {
        if(!request.HasFormContentType)
        {
            return Results.BadRequest("not form content " + request.ContentType);
        }

        var fileName = request.Form["name"];
        _lastUploadName = fileName;
        var form = await request.ReadFormAsync();
        var fileForm = form.Files["file"];

        if(fileForm == null || fileForm.Length == 0)
        {
            return Results.BadRequest("file not exits");
        }

        if(!Directory.Exists("save"))
        {
            Directory.CreateDirectory("save");
        }

        fileName = "save/" + fileName;
        await using(var stream = fileForm.OpenReadStream())
        {
            await using(var file = File.Create(fileName))
            {
                await stream.CopyToAsync(file);
            }    
        }

        return Results.Ok();
    }
}
