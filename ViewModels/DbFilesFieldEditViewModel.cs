using System.Collections.Generic;

namespace DbFilesField.ViewModels {

    public class DbFilesFieldEditViewModel {
        public string Name { get; set; }
        public string Hint { get; set; }
        public bool IsRequired { get; set; }
        public string Value { get; set; }
        public bool AllowMultiple { get; set; }
        public List<DbFilesFieldViewModel> DbFilesViewModel { get; set; }
        public int MaxKb { get; set; }

        public DbFilesFieldEditViewModel() {
            DbFilesViewModel = new List<DbFilesFieldViewModel> {new DbFilesFieldViewModel()};
        }
    }

}