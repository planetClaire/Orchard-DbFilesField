using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace DbFilesField.Fields
{
    public class DbFilesField : ContentField
    {
        public int IdFileUpload
        {
            get { return Storage.Get<int>(); }
            set { Storage.Set(value); }
        }
    }
}