using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestChunkUpload.API.Models
{
    public class ChunkMetaData
    {
        public int chunkIndex { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public int fileSize { get; set; }
        public string fileUid { get; set; }
        public int totalChunks { get; set; }
    }
}
