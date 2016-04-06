using System.Collections.Generic;
using System.Linq;
using DbFilesField.Models;
using DbFilesField.ViewModels;
using Orchard.Data;

namespace DbFilesField.Services
{
    public class DbFilesService : IDbFilesService
    {
        private readonly IRepository<FileUploadRecord> _fileUploadRepository;

        public DbFilesService(IRepository<FileUploadRecord> fileUploadRepository) {
            _fileUploadRepository = fileUploadRepository;
        }

        public List<DbFilesFieldViewModel> GetFilesForField(string fieldName, int idContentItem) {
            var viewModel = new List<DbFilesFieldViewModel>();
            foreach (var record in _fileUploadRepository.Table.Where(f => f.IdContent == idContentItem && f.FieldName == fieldName))
            {
                viewModel.Add(new DbFilesFieldViewModel
                {
                    FileName = record.FileName,
                    IdFileUpload = record.Id
                });
            }
            return viewModel;
        }
    }
}