using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdVoiceRecognizer.Models
{
    public class AudioFileAnalysis
    {
        public string? id { get; set; }
        public string? fileName { get; set; }
        public string? AnalysisResult { get; set; }
        public double ConfidenceScore { get; set; }
        public DateTime AnalysisTime { get; set; }
    }
}