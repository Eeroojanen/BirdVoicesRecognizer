using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdVoiceRecognizer.Models
{
    public class AudioFile
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public DateTime UploadedAt { get; set; }
        public string AnalysisResult { get; set; }
    }

}
