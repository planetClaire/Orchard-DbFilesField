using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbFilesField.Models;
using DbFilesField.Services;
using DbFilesField.Settings;
using DbFilesField.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Localization;

namespace DbFilesField.Drivers
{
    public class DbFilesFieldDriver : ContentFieldDriver<Fields.DbFilesField> {
        private readonly IRepository<FileUploadRecord> _fileUploadRepository;
        private readonly IDbFilesService _dbFilesService;

        private const string TemplateName = "Fields/DbFilesField.Edit";

        public DbFilesFieldDriver(IRepository<FileUploadRecord> fileUploadRepository, IDbFilesService dbFilesService) {
            _fileUploadRepository = fileUploadRepository;
            _dbFilesService = dbFilesService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.DbFilesField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_DbFilesField", field.Name, () => shapeHelper.Fields_DbFilesField(ViewModel: _dbFilesService.GetFilesForField(field.Name, part.ContentItem.Id)));
        }

        protected override DriverResult Editor(ContentPart part, Fields.DbFilesField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<DbFilesFieldSettings>();

            var viewModel = new DbFilesFieldEditViewModel {
                Name = field.Name,
                Hint = settings.Hint,
                IsRequired = settings.Required,
                AllowMultiple = settings.AllowMultiple,
                DbFilesViewModel = _dbFilesService.GetFilesForField(field.Name, part.ContentItem.Id)
            };

            return ContentShape("Fields_DbFilesField_Edit", field.Name, () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, Fields.DbFilesField field, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new DbFilesFieldEditViewModel();
            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {
                var request = ((Controller) updater).Request;
                var settings = field.PartFieldDefinition.Settings.GetModel<DbFilesFieldSettings>();
                var keyName = "DbFiles-" + field.Name;
                var postedFiles = new List<HttpPostedFileBase>();
                for (var i = 0; i < request.Files.Count; i++) {
                    if (request.Files.GetKey(i).Equals(keyName)) {
                        var postedFile = request.Files[i];
                        if (postedFile != null && postedFile.InputStream.Length > 0) {
                            postedFiles.Add(postedFile);
                        }
                    }
                }
                if (settings.Required) {
                    var existingFiles = _dbFilesService.GetFilesForField(field.Name, part.ContentItem.Id);
                    if (!existingFiles.Any() && !postedFiles.Any()) {
                        updater.AddModelError(GetPrefix(field, part), T("{0} is required.", field.DisplayName));
                        return Editor(part, field, shapeHelper);
                    }
                }
                try {
                    foreach (var postedFile in postedFiles) {
                        var document = new byte[postedFile.ContentLength];
                        postedFile.InputStream.Read(document, 0, postedFile.ContentLength);
                        var record = new FileUploadRecord {
                            FileData = document,
                            IdContent = part.ContentItem.Id,
                            FieldName = field.Name,
                            FileName = Path.GetFileName(postedFile.FileName),
                            ContentType = postedFile.ContentType
                        };
                        _fileUploadRepository.Create(record);
                    }
                }
                catch (Exception ex) {
                    updater.AddModelError(GetPrefix(field, part), T("Failed to save {0} - {1}.", field.DisplayName, ex.Message));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        //protected override void Importing(ContentPart part, DateRangeField field, ImportContentContext context)
        //{
        //    context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        //}

        //protected override void Exporting(ContentPart part, DateRangeField field, ExportContentContext context)
        //{
        //    context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        //}

        //protected override void Describe(DescribeMembersContext context)
        //{
        //    context
        //        .Member(null, typeof(string), T("Value"), T("The date values of the field."))
        //        .Enumerate<DateRangeField>(() => field => field.Value);
        //}
    }
}
