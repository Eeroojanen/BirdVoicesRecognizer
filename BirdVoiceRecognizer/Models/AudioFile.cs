using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdVoiceRecognizer.Models
{
    public class AudioFile
    {
        public string id { get; set; }
        public string fileName { get; set; }
        public byte[] content { get; set; }
        public string blobUrl {get; set;}
        public DateTime uploadedAt { get; set; }
    }

}
