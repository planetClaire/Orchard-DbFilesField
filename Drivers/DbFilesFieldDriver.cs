using System;
using System.IO;
using System.Web.Mvc;
using DbFilesField.Models;
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
        private const string TemplateName = "Fields/DbFilesField.Edit";

        public DbFilesFieldDriver(IRepository<FileUploadRecord> fileUploadRepository) {
            _fileUploadRepository = fileUploadRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.DbFilesField field, string displayType, dynamic shapeHelper)
        {
            var viewModel = new DbFilesFieldViewModel();
            var record = _fileUploadRepository.Get(field.IdFileUpload);
            if (record != null) {
                viewModel.IdFileUpload = field.IdFileUpload;
                viewModel.FileName = record.FileName;
            }
            return ContentShape("Fields_DbFilesField", field.Name, () => shapeHelper.Fields_DbFilesField(ViewModel: viewModel));
        }

        protected override DriverResult Editor(ContentPart part, Fields.DbFilesField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<DbFilesFieldSettings>();

            var viewModel = new DbFilesFieldEditViewModel
            {
                Name = field.Name,
                Hint = settings.Hint,
                IsRequired = settings.Required,
                AllowMultiple = settings.AllowMultiple
            };

            var record = _fileUploadRepository.Get(field.IdFileUpload);
            if (record != null) {
                viewModel.DbFilesViewModel = new DbFilesFieldViewModel {
                    IdFileUpload = field.IdFileUpload,
                    FileName = record.FileName
                };
            }

            return ContentShape("Fields_DbFilesField_Edit", field.Name, () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, Fields.DbFilesField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new DbFilesFieldEditViewModel();

            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {

                // todo validate required
                //if (settings.Required && string.IsNullOrEmpty(viewModel.Value)) {
                //    updater.AddModelError(GetPrefix(field, part), T("{0} is required.", field.DisplayName));
                //}
                //else {
                    try {
                        var postedFile = ((Controller)updater).Request.Files["DbFiles-" + field.Name];
                        if (postedFile != null) {
                            var filename = Path.GetFileName(postedFile.FileName);
                            var contentType = postedFile.ContentType;
                            byte[] document = new byte[postedFile.ContentLength];
                            postedFile.InputStream.Read(document, 0, postedFile.ContentLength);
                            var record = new FileUploadRecord {
                                FileData = document,
                                IdContent = part.ContentItem.Id,
                                FileName = filename,
                                ContentType = contentType
                            };
                            _fileUploadRepository.Create(record);
                            field.IdFileUpload = record.Id;
                        }
                    }
                    catch (Exception ex){
                        updater.AddModelError(GetPrefix(field, part), T("Failed to save {0} - {1}.", field.DisplayName, ex.Message));
                    }
                //}
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
