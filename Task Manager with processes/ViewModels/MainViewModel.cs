using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Task_Manager_with_processes.Command;

namespace Task_Manager_with_processes.ViewModels
{
    public class MainViewModel
    {
        public MainWindow MainWindows { get; set; }
     
        public RelayCommand SelectProcessCommand { get; set; }

        public RelayCommand EndProcessCommand { get; set; }

        public RelayCommand CreateProcessCommand { get; set; }

        public RelayCommand AddBlackListCommand { get; set; }

        public RelayCommand SearchCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<string> BlackList { get; set; } = new ObservableCollection<string>();
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        private Process _Process;
        public Process Process { get { return _Process; } set { _Process = value; OnPropertyChanged(); } }

        private ObservableCollection<Process> allProcess;

        public ObservableCollection<Process> AllProcess
        {
            get { return allProcess; }
            set { allProcess = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick; ;
            timer.Interval = TimeSpan.FromSeconds(1);
            AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
            timer.Start();
            EndProcessCommand = new RelayCommand((sender) =>
            {
                try
                {


                    if (MainWindows.AddTaskTxt.Text == string.Empty)
                    {
                        foreach (var item in AllProcess)
                        {
                            var i = MainWindows.proceslistview.SelectedItem as Process;
                            if (item.ProcessName == i.ProcessName)
                            {
                                if (!item.WaitForExit(1000))
                                {
                                    MessageBox.Show(item.StartTime.ToLongTimeString());
                                    for (int J = 0; J < 100; J++)
                                    {
                                        if (!item.HasExited) item.Kill();

                                    }
                                }
                            }
                        }
                        AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
                    }
                }
                catch (Exception)
                {


                }
            });


            SearchCommand = new RelayCommand((sender) =>
            {
                try
                {
                    MainWindows.proceslistview.ItemsSource = null;

                    if (string.IsNullOrEmpty(MainWindows.SearchTXT.Text) == false)
                    {
                        MainWindows.proceslistview.ItemsSource = null;
                        MainWindows.proceslistview.Items.Clear();

                        foreach (var item in AllProcess)
                        {
                            if (item.ProcessName.StartsWith(MainWindows.SearchTXT.Text))
                            {
                                MainWindows.proceslistview.Items.Add(item);
                            }
                            MainWindows.proceslistview.ItemsSource = null;

                        }

                        AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
                    }

                    else
                    {
                        MainWindows.proceslistview.Items.Clear();

                        foreach (var item in AllProcess)
                        {

                            MainWindows.proceslistview.Items.Add(item);

                        }
                        AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
                    }
                }
                catch (Exception)
                {


                }

            });
            CreateProcessCommand = new RelayCommand((sender) =>
            {
              

                try
                {
                    
                      
                        Process p = new Process();
                        p.StartInfo.FileName = MainWindows.AddTaskTxt.Text;
                        p.Start();
                        AllProcess.Add(p);
                        if (BlackList.Any(x => x == MainWindows.AddTaskTxt.Text))
                        {
                            Thread.Sleep(3000);
                            AllProcess.Remove(p);
                            p.Kill();
                        }

                    AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
                }
                catch (Exception)
                {

                }

                MainWindows.AddTaskTxt.Text = string.Empty;
            });
            AddBlackListCommand = new RelayCommand((sender) =>
            {
                MainWindows.blacklistbox.Items.Add(MainWindows.BlacklistTxt.Text);
                if (!string.IsNullOrWhiteSpace(MainWindows.BlacklistTxt.Text))
                {

                    if (!BlackList.Any(x => x == MainWindows.BlacklistTxt.Text))
                    {
                        BlackList.Add(MainWindows.BlacklistTxt.Text);
                        MainWindows.BlacklistTxt.Text = "";
                    }
                }
                
            });
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
           
        }
    }
}
