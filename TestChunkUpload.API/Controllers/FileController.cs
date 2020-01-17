using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestChunkUpload.API.Models;

namespace TestChunkUpload.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public static readonly string UPLOADS_PATH = Path.Combine(Environment.CurrentDirectory, "Uploads");

        [HttpGet("List")]
        public async Task<IActionResult> ListAsync(CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                if (!Directory.Exists(UPLOADS_PATH)) Directory.CreateDirectory(UPLOADS_PATH);

                var id = 1;
                var files = Directory.GetFiles(UPLOADS_PATH).Select(x => new { FileID = id++, FileName = Path.GetFileName(x) });
                return Ok(files);
            }, ct);            
        }

        [HttpGet("Download/{file}")]
        public async Task<IActionResult> DownloadAsync([FromRoute] string file, CancellationToken ct)
        {
            return await Task.Run<IActionResult>(() =>
            {
                if (!Directory.Exists(UPLOADS_PATH)) Directory.CreateDirectory(UPLOADS_PATH);

                var filePath = Path.Combine(UPLOADS_PATH, file);
                if (System.IO.File.Exists(filePath))
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return this.File(fileStream, "application/octet-stream");
                }

                return NotFound(new { Error = true, Message = "File doesn't exist." });
            }, ct);
        }

        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveAsync([FromForm]string fileNames, CancellationToken ct)
        {
            return await Task.Run<IActionResult>(() =>
            {
                if (!Directory.Exists(UPLOADS_PATH)) Directory.CreateDirectory(UPLOADS_PATH);

                var filePath = Path.Combine(UPLOADS_PATH, fileNames);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                else
                    return BadRequest(new { Error = true, Message = "File doesn't exists." });
                return Ok();
            }, ct);            
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> PostAsync([FromForm] FormData data, CancellationToken ct)
        {
            if (!Directory.Exists(UPLOADS_PATH)) Directory.CreateDirectory(UPLOADS_PATH);
            var tmpFile = Path.Combine(UPLOADS_PATH, $"{data.Metadata.fileUid}.tmp");

            if (data.Metadata.chunkIndex == 0)
            {
                if (!System.IO.File.Exists(tmpFile))
                    await System.IO.File.Create(tmpFile).DisposeAsync();
                else
                    return BadRequest();
            }

            foreach (var file in data.Files)
            {
                using (var fileStream = new FileStream(tmpFile, FileMode.Append))
                {
                    await file.CopyToAsync(fileStream, ct);
                    await fileStream.FlushAsync();
                }
            }

            if (data.Metadata.chunkIndex == data.Metadata.totalChunks - 1)
            {
                System.IO.File.Move(tmpFile, Path.Combine(UPLOADS_PATH, Path.GetFileName(data.Metadata.fileName)));
            }

            return Ok();
        }

    }
}