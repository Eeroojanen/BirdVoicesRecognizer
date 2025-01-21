using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdVoiceRecognizer.Models
{
    public class AudioFile
    {
       public string? id { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }
        public string? BlobUrl { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
