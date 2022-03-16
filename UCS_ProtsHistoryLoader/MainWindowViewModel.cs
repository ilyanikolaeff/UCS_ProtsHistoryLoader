using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UCS_ProtsHistoryLoader
{
    class MainWindowViewModel : ViewModelBase
    {
        private IDialogService _dialogService = new DefaultDialogService();
        public string IpAddress
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ServerName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ConfigPath
        {
            get { return GetValue<string>(); }
            set => SetValue(value);
        }

        public ICommand LoadConfigCommand
        {
            get; private set;
        }

        public AsyncCommand RunCommand { get; private set; }

        public double ProgressValue
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public string ProgressState
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public DateTime StartTime
        {
            get => GetValue<DateTime>();
            set => SetValue(value);
        }

        public DateTime EndTime
        {
            get => GetValue<DateTime>();
            set => SetValue(value);
        }

        public MainWindowViewModel()
        {
#if DEBUG
            IpAddress = "10.2.180.10";
            ServerName = "Infinity.OPCHDAServer";
#endif

            LoadConfigCommand = new DelegateCommand(LoadConfig);
            //RunCommand = new AsyncCommand(Run, CanRun);
            RunCommand = new AsyncCommand(async () => await Task.Run(Run), CanRun);

            StartTime = DateTime.Now.AddDays(-1);
            EndTime = DateTime.Now;
        }

        private void LoadConfig()
        {
            if (_dialogService.ShowOpenFileDialog())
            {
                try
                {
                    ConfigPath = _dialogService.FilePath;
                    ConfigurationProvider.GetInstance().Load(ConfigPath);
                    var config = ConfigurationProvider.GetInstance().ProtConfigs;
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"Error loading config:\n{ex}");
                }
            }
        }

        private bool CanRun()
        {
            return ConfigPath != null && ConfigurationProvider.GetInstance().IsInitialized;
        }

        private async Task Run()
        {
            try
            {
                var historyLoader = new HistoryLoader(IpAddress, ServerName);
                historyLoader.Init();
                historyLoader.ProgressChanged += HistoryLoader_ProgressChanged;
                var startTime = StartTime;
                var endTime = EndTime;

                var results = new List<Protection>();
                var protsConfig = ConfigurationProvider.GetInstance().ProtConfigs;
                int counter = 1;
                int count = protsConfig.Count();
                foreach (var protConfig in protsConfig)
                {
                    var currentResults = await historyLoader.Load(protConfig, startTime, endTime);
                    results.AddRange(currentResults);
                    ProgressValue = (double)counter++ / count * 100;
                }
                ProgressState += "\nНачало экспорта в файл для Сивкова (SivkovKakSam.txt)";
                ExportHelper.Export(results);
                ProgressState += "\nКонец экспорта в файл для Сивкова (SivkovKakSam.txt)";
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Error run:\n{ex}");
            }
        }

        private void HistoryLoader_ProgressChanged(object sender, NotifyEventArgs e)
        {
            ProgressState += $"{e.Time} {e.Messsage}\n";
        }
    }
}
