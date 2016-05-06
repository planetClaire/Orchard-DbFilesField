using System.IO;
using System.Linq;
using System.Web.Mvc;
using DbFilesField.Models;
using DbFilesField.Settings;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Security;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf.IO;

namespace DbFilesField.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IRepository<FileUploadRecord> _fileUploadRepository;
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;
        private readonly IWorkContextAccessor _workContext;

        public ILogger Logger { get; set; }

        public DownloadController(IRepository<FileUploadRecord> fileUploadRepository, IContentManager contentManager, IAuthorizer authorizer, IWorkContextAccessor workContext)
        {
            _fileUploadRepository = fileUploadRepository;
            _contentManager = contentManager;
            _authorizer = authorizer;
            _workContext = workContext;
            Logger = NullLogger.Instance;
        }

        public ActionResult Index(int id) {
            var fileRecord = _fileUploadRepository.Get(id);
            if (fileRecord == null) {
                return new HttpNotFoundResult();
            }
            
            var contentItem = _contentManager.Get(fileRecord.IdContent);
            if (contentItem == null || !_authorizer.Authorize(Permissions.ViewContent, contentItem)) {
                return new HttpUnauthorizedResult();
            }

            Response.AddHeader("Content-type", "application/x-download");
            Response.AddHeader("Content-Disposition", string.Format("\"attachment; filename=\"{0}\"", fileRecord.FileName));

            var fileContents = fileRecord.FileData;

            var currentUser = _workContext.GetContext().CurrentUser;
            if (currentUser == null || !fileRecord.ContentType.Equals("application/pdf"))
            {
                return FileResult(fileContents);
            }
            
            var part = contentItem.Parts.FirstOrDefault(p => p.Has(typeof(Fields.DbFilesField), fileRecord.FieldName));
            if (part == null) {
                Logger.Error(string.Format("Could not find ContentPart for FileUploadRecord {0} - served file without annotation.", fileRecord.Id));
                return FileResult(fileContents);
            }
            
            var field = part.Fields.FirstOrDefault(f => f.Name == fileRecord.FieldName);
            if (field == null) {
                Logger.Error(string.Format("Could not find Field for FileUploadRecord {0} - served file without annotation.", fileRecord.Id));
                return FileResult(fileContents);
            }
            
            var fieldSettings = field.PartFieldDefinition.Settings.TryGetModel<DbFilesFieldSettings>();
            if (fieldSettings == null) {
                Logger.Error(string.Format("Could not find Field Settings for FileUploadRecord {0} - served file without annotation.", fileRecord.Id));
                return FileResult(fileContents);
            }

            if (!fieldSettings.AnnotateFile || string.IsNullOrEmpty(fieldSettings.AnnotateFormatString)) {
                return FileResult(fileContents);
            }
            
            var document = PdfReader.Open(new MemoryStream(fileRecord.FileData), PdfDocumentOpenMode.Modify);
            var page = document.Pages[0];
            var tf = new XTextFormatter(XGraphics.FromPdfPage(page)) {
                Alignment = XParagraphAlignment.Right
            };
            var font = new XFont("Arial", 12, XFontStyle.Regular);

            try {
                tf.DrawString(string.Format(fieldSettings.AnnotateFormatString, currentUser.Email), font, XBrushes.Black, new XRect(0, 5, page.Width - 10, page.Height - 5));
            }
            catch (System.FormatException ex) {
                Logger.Error(ex, string.Format("Invalid AnnoteFormatString on FileUploadRecord {0} - served file without annotation.", fileRecord.Id));
                return FileResult(fileContents);
            }
            
            using (var stream = new MemoryStream()) {
                document.Save(stream, true);
                fileContents = stream.ToArray();
            }
            Response.BinaryWrite(fileContents);
            Response.Flush();
            Response.End();
            return null;
        }

        private ActionResult FileResult(byte[] fileContents) {
            HttpContext.Response.OutputStream.Write(fileContents, 0, fileContents.Length);
            Response.Flush();
            Response.End();
            return null;
        }
    }
}