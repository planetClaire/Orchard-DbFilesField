using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace DbFilesField.Settings
{
    public class DbFilesFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "DbFilesField")
            {
                var model = definition.Settings.GetModel<DbFilesFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "DbFilesField")
            {
                yield break;
            }

            var model = new DbFilesFieldSettings();
            if (updateModel.TryUpdateModel(model, "DbFilesFieldSettings", null, null))
            {
                builder.WithSetting("DbFilesFieldSettings.Hint", model.Hint);
                builder.WithSetting("DbFilesFieldSettings.Required", model.Required.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("DbFilesFieldSettings.AllowMultiple", model.AllowMultiple.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("DbFilesFieldSettings.MaxKb", model.MaxKb.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("DbFilesFieldSettings.AnnotateFile", model.AnnotateFile.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("DbFilesFieldSettings.AnnotateFormatString", model.AnnotateFormatString);

                yield return DefinitionTemplate(model);
            }
        }
    }
}