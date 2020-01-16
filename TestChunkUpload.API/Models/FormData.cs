using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestChunkUpload.API.Models
{
    public class FormData
    {
        [FromForm(Name = "files")]
        public IList<IFormFile> Files { get; set; }

        [FromForm(Name = "metadata")]
        public string MetadataJson { get => JsonConvert.SerializeObject(Metadata); set => Metadata = JsonConvert.DeserializeObject<ChunkMetaData>(value); }

        public ChunkMetaData Metadata { get; set; } = new ChunkMetaData();
    }
}
