using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestChunkUpload.API.Models
{
    public class FileData : Dictionary<string, FileData.Data>
    {
        public void Add(string key, int size)
        {
            if (!this.ContainsKey(key))
                this.Add(key, new Data(size));
        }

        public class Data
        {
            public int Count { get; set; }
            public FilePart[] Parts { get; set; }

            public Data(int size)
            {
                this.Count = 0;
                this.Parts = new FilePart[size];
            }
        }

        public class FilePart
        {
            public string PartName { get; set; }
        }
    }    
}
