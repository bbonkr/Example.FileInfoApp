using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Example.FileInfoApp;

/// <summary>
/// File Information 
/// </summary>
[Area("api")]
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion(1.0)]
public class FilesController : ControllerBase
{
    /// <summary>
    /// Upload files 
    /// </summary>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost()]
    public async Task<ActionResult<FilesModel>> PostFiles(IEnumerable<IFormFile> files, CancellationToken cancellationToken = default)
    {
        FilesModel result = new();
        List<FileInformationModel> resultItems = new();

        if (files.Any())
        {
            foreach (var file in files)
            {
                using var outStream = new MemoryStream();
                using var stream = file.OpenReadStream();

                stream.Position = 0;

                await stream.CopyToAsync(outStream, cancellationToken);
                await stream.FlushAsync(cancellationToken);
                stream.Close();

                FileInformationModel fileInformationModel = new()
                {
                    Name = file.FileName,
                    ContentType = file.ContentType,
                    SizeValue = outStream.Length,
                };

                resultItems.Add(fileInformationModel);
            }

            result.Items = resultItems;
        }

        return Ok(result);
    }
}
