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
                var filePath = Path.Combine(UPLOADS_PATH, file);
                if (System.IO.File.Exists(filePath))
                {
                    return new FileContentResult(System.IO.File.ReadAllBytes(filePath), "application/octet-stream");
                }

                return NotFound(new { Error = true, Message = "File doesn't exist." });
            }, ct);
        }

        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveAsync([FromForm]string fileNames, CancellationToken ct)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var filePath = Path.Combine(UPLOADS_PATH, fileNames);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                else
                    return BadRequest(new { Error = true, Message = "File doesn't exists." });
                return Ok();
            }, ct);            
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> PostAsync([FromForm] FormData data, [FromServices] UploadedData uploadedData, CancellationToken ct)
        {
            try
            {
                if (!Directory.Exists(UPLOADS_PATH)) Directory.CreateDirectory(UPLOADS_PATH);

                if (data.Metadata.chunkIndex == 0)
                {
                    var filename = Path.GetFileName(data.Metadata.fileName);
                    var filepath = Path.Combine(UPLOADS_PATH, filename);

                    if (System.IO.File.Exists(filepath)) return BadRequest(new { Error = true, Message = "File already exists." });

                    uploadedData.Add(data.Metadata.fileUid, new FileStream(filepath, FileMode.OpenOrCreate));
                }

                foreach (var file in data.Files)
                {
                    await file.CopyToAsync(uploadedData[data.Metadata.fileUid], ct);
                    await uploadedData[data.Metadata.fileUid].FlushAsync();
                }

                if (data.Metadata.chunkIndex == data.Metadata.totalChunks - 1)
                {
                    await uploadedData[data.Metadata.fileUid].DisposeAsync();
                    uploadedData.Remove(data.Metadata.fileUid);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = true, ex.Message, ex.InnerException });
            }
        }

    }
}