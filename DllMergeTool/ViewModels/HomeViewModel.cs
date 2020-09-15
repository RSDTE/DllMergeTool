using Caliburn.Micro;
using DllMergeTool.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace DllMergeTool.ViewModels
{
    public class HomeViewModel : Screen, IShell
    {
        private ObservableCollection<string> _filesPath = new ObservableCollection<string>();
        public ObservableCollection<string> Items
        {
            get
            {
                return _filesPath;
            }

            set
            {
                _filesPath = value;
                NotifyOfPropertyChange(() => Items);

            }
        }


        private string _mainFilePath;
        public string MainFilePath
        {
            get
            {
                return _mainFilePath;
            }
            set
            {
                _mainFilePath = value;
                NotifyOfPropertyChange(() => MainFilePath);
            }
        }

        public void OnSelectedMainFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.dll)|*.dll";
            var ret = fileDialog.ShowDialog()??false;
            if (ret)
            {
                MainFilePath = fileDialog.FileName;
            }

        }


        public void OnSelectedOtherFiles()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.dll)|*.dll";
            fileDialog.Multiselect = true;
            var ret = fileDialog.ShowDialog() ?? false;
            if (ret)
            {
                foreach (var item in fileDialog.FileNames)
                {
                    Items.Add(item);
                }
            }
        }

        private string selectedItem;
        public void SelectedItem(RoutedEventArgs e)
        {

        }

        public void RemoveSelectedItem()
        {

        }

        private string _exportPath;
        public string ExportPath
        {
            get
            {
                return _exportPath;
            }
            set
            {
                _exportPath = value;
                NotifyOfPropertyChange(() => ExportPath);
            }
        }

        private string _exportFileName;
        private readonly IWindowManager windowManager;

        public string ExportFileName
        {
            get
            {
                return _exportFileName;
            }
            set
            {
                _exportFileName = value;
                NotifyOfPropertyChange(() => ExportFileName);
            }
        }
        public void OnSelectedFolder()
        {

            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK){
                ExportPath = dlg.SelectedPath;
            }
        }


        public HomeViewModel([Import]IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public string Title { get; set; }

        public async Task OnExport()
        {
            var view = (MessageViewModel)IoC.Get<IModule>();
            view.Title = "Info";
            var merge = new ILMerging.ILMerge();
            if(Items is null || MainFilePath is null || ExportFileName is null || ExportPath is null || Items.Count < 1 || MainFilePath.Length < 1 || ExportPath.Length < 1 || ExportFileName.Length < 1)
            {
                view.Content = "配置错误";
                windowManager.ShowDialog(view);
                return;
            }

            var data = Items.ToList<string>();
            data.Add(MainFilePath);
            merge.SetInputAssemblies(data.ToArray());
            var outpath = Path.Combine(ExportPath, ExportFileName);
            FileInfo fileInfo = new FileInfo(outpath);
            if (!fileInfo.Extension.Contains("dll"))
            {
                MessageBox.Show("the extension of the output file name must be dll.");
                return;
            }
            merge.OutputFile = outpath;
            merge.SetTargetPlatform("v4", string.Empty);
            merge.TargetKind = ILMerging.ILMerge.Kind.SameAsPrimaryAssembly;

            var info = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    merge.Merge();
                    info = "successfully";
                    windowManager.ShowDialog(view);
                }
                catch(Exception ex)
                {
                    info = ex.Message;
                }

            });
            view.Content = info;
            windowManager.ShowDialog(view);
        }




    }
}
