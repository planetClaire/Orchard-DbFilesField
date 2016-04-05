using DbFilesField.Attributes;

namespace DbFilesField.Models
{
    public class FileUploadRecord
    {
        public virtual int Id { get; set; }
        public virtual int IdContent { get; set; }
        public virtual string FileName { get; set; }
        public virtual string ContentType { get; set; }
        [BinaryLengthMax]
        public virtual byte[] FileData { get; set; }

    }
}