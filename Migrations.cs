using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DbFilesField
{
    public class Migrations : DataMigrationImpl
    {
        public int Create() {
            SchemaBuilder.CreateTable("FileUploadRecord", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("IdContent")
                .Column<string>("FileName")
                .Column<string>("ContentType")
                .Column<byte[]>("FileData", c => c.Unlimited().WithType(DbType.Binary))
                );

            return 6;
        }

        public int UpdateFrom6() {
            SchemaBuilder.AlterTable("FileUploadRecord", table => table.AddColumn<string>("FieldName"));

            return 9;
        }

        public int UpdateFrom9() {
            ContentDefinitionManager.AlterPartDefinition("DBFilesFieldWidgetPart", part => part
                .Attachable()
                .WithField("DBFiles", field => field
                    .OfType("DbFilesField")
                ));

            ContentDefinitionManager.AlterTypeDefinition("DBFilesFieldWidget", type => type
                .WithPart("DBFilesFieldWidgetPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")
                .Securable());

            return 10;
        }
    }

}