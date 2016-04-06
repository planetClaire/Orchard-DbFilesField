using System.Collections.Generic;
using DbFilesField.ViewModels;
using Orchard;

namespace DbFilesField.Services
{
    public interface IDbFilesService : IDependency
    {
        List<DbFilesFieldViewModel> GetFilesForField(string fieldName, int idContentItem);
        void DeleteFile(string id);
    }
}