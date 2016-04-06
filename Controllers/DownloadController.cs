using System.Web.Mvc;
using DbFilesField.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Data;
using Orchard.Security;

namespace DbFilesField.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IRepository<FileUploadRecord> _fileUploadRepository;
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;

        public DownloadController(IRepository<FileUploadRecord> fileUploadRepository, IContentManager contentManager, IAuthorizer authorizer) {
            _fileUploadRepository = fileUploadRepository;
            _contentManager = contentManager;
            _authorizer = authorizer;
        }

        public ActionResult Index(int id) {
            var fileRecord = _fileUploadRepository.Get(id);
            if (fileRecord == null) {
                return HttpNotFound();
            }
            
            var contentItem = _contentManager.Get(fileRecord.IdContent);
            if (contentItem == null || !_authorizer.Authorize(Permissions.ViewContent, contentItem)) {
                return new HttpUnauthorizedResult();
            }

            HttpContext.Response.AddHeader("Content-type", fileRecord.ContentType);
            HttpContext.Response.AddHeader("Content-Disposition", string.Format("\"attachment; filename=\"{0}\"", fileRecord.FileName));
            HttpContext.Response.OutputStream.Write(fileRecord.FileData, 0, fileRecord.FileData.Length);
            HttpContext.Response.Flush();
            HttpContext.Response.End();
            return null;
        }
    }
}