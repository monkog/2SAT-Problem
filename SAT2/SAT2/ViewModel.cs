using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;

namespace SAT2
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Private Members
        private string _filePath;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets or sets the path of the xml file with 2 sat problem.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath == value) return;
                _filePath = value;
                OnPropertyChanged("FilePath");
            }
        }
        #endregion Public Properties
        #region Private Methods
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
        #region Commands
        private DelegateCommand _browsePathCommand;
        public DelegateCommand BrowsePathCommand { get { return _browsePathCommand ?? (_browsePathCommand = new DelegateCommand(BrowsePathExecuted)); } }
        private void BrowsePathExecuted()
        {
            OpenFileDialog dialogWindow = new OpenFileDialog();
            dialogWindow.Filter = "XML(*.xml|*.xml";
            if (dialogWindow.ShowDialog() == true)
            {
                FilePath = dialogWindow.FileName;
                SolveSatProblemCommand.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _solveSatProblemCommand;
        public DelegateCommand SolveSatProblemCommand { get
        {
            return _solveSatProblemCommand ??
                   (_solveSatProblemCommand = new DelegateCommand(SolveSatProblemExecuted, SolveSatProblemCanExecute));
        } }
        private bool SolveSatProblemCanExecute()
        {
            return !string.IsNullOrEmpty(FilePath);
        }
        private void SolveSatProblemExecuted()
        {
            Sat2Problem problem = new Sat2Problem();
            problem.Run(FilePath);
        }
        #endregion Commands
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

