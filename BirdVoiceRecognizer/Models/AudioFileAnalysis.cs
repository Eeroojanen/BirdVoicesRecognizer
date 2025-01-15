using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdVoiceRecognizer.Models
{
    public class AudioFileAnalysis
    {
        public string fileName { get; set; }
        public string analysisResult { get; set; }
        public string analysisMessage { get; set; }
    }
}