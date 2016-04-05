using System.Data;
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

    }
}