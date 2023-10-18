


using System.Net;
using System.Net.Http.Headers;

namespace PizzaStore;

public class Uploader
{
    static Dictionary<string, string> _dicLatestUploadName = new Dictionary<string, string>();

    internal static async Task ComprareLog(HttpContext context)
    {
        var msg = context.Request.Form["msg"];
        var path = context.Request.Form["path"];

        await File.WriteAllTextAsync(path, msg);
    }

    internal static async Task uploadError(HttpContext context)
    {
        var msg = context.Request.Form["gitGuid"];
        var path = context.Request.Form["battleGuid"];

        if(!Directory.Exists("error"))
        {
            Directory.CreateDirectory("error");
        }

        await File.WriteAllTextAsync($"error/{path}", msg);
    }

    async internal static Task Download(HttpContext context)    
    {
        var name = context.Request.Form["name"];
        var platform = context.Request.Form["platform"];

        context.Response.ContentType = "image/png";
        using (var fileStream = File.OpenRead($"save/{platform}/{name}")) 
        {
            await fileStream.CopyToAsync(context.Response.Body);
        }
    }

    internal static IResult GetLatestPlayback(HttpRequest request)
    {
        var platform = request.Form["platform"];
        if(!_dicLatestUploadName.TryGetValue(platform, out var path))
        {
            DirectoryInfo d = new DirectoryInfo($"save/{platform}");
            if(!d.Exists)
            {
                return Results.Ok("");
            }

            List<FileInfo> files = d.GetFiles().ToList(); 
            files.Sort((m, n)=>{return m.CreationTime.CompareTo(n.CreationTime);});
            if(files.Count > 0)
            {
                path = files[0].Name;
                _dicLatestUploadName.Add(platform, path);
            }
        }

        return Results.Ok(path);
    }

    internal static async Task GetUnSyncMessage(HttpContext context)
    {
        var msg = context.Request.Form["msg"];
        var error = context.Request.Form["errorMsg"];

        await File.AppendAllLinesAsync("unsync.txt", new string[]{msg, error});
    }

    async internal static Task<IResult> Upload(HttpRequest request)
    {
        if(!request.HasFormContentType)
        {
            return Results.BadRequest("not form content " + request.ContentType);
        }

        var fileName = request.Form["name"];
        var platform = request.Form["platform"];
        
        _dicLatestUploadName[platform] = fileName;
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
        if(!Directory.Exists($"save/{platform}"))
        {
            Directory.CreateDirectory($"save/{platform}");
        }

        fileName = $"save/{platform}/{fileName}";
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
