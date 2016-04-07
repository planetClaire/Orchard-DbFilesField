using System.ComponentModel.DataAnnotations;

namespace DbFilesField.Settings
{
    public class DbFilesFieldSettings
    {
        public string Hint { get; set; }
        public bool Required { get; set; }
        public bool AllowMultiple { get; set; }
        [Required]
        public int MaxKb { get; set; }
        public bool AnnotateFile { get; set; }
        public string AnnotateFormatString { get; set; }
    }
}