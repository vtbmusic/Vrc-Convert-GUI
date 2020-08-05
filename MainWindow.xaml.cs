using ControlzEx.Standard;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vrc_Lyric_Format_Convert_GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private string sourceFormat = "";
        private string jsonIndent = "2";
        // 非递归模式
        private string sourcePath = "";
        private string outputPath = "";
        private string deOutPutPath = "";
        // 递归模式
        private bool Recursion = false;
        private string sourceDirPath = "";
        private string outputDirPath = "";

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("bin/convert.exe") != true)
            {
                MessageBox.Show("找不到关键文件 bin/convert.exe", "发生错误 - 程序即将退出");
                Environment.Exit(0);
            }
            // 初始化 RadioButton 点击事件
            txtRadio.Checked += new RoutedEventHandler(radio_Checked);
            mlrcRadio.Checked += new RoutedEventHandler(radio_Checked);
            assRadio.Checked += new RoutedEventHandler(radio_Checked);
            txtRadio.Unchecked += new RoutedEventHandler(radio_Unchecked);
            mlrcRadio.Unchecked += new RoutedEventHandler(radio_Unchecked);
            assRadio.Unchecked += new RoutedEventHandler(radio_Unchecked);
        }

        #region 运行转换
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            jsonIndent = JsonIndent.Text;

            if (Recursion)
            {
                if (sourceFormat == "")
                {
                    this.ShowMessageAsync("运行失败", "你还没有输入完参数呢!怎么跑");
                }
                else if (sourceDirPath == "")
                {
                    this.ShowMessageAsync("运行失败", "你还没有输入完参数呢!怎么跑");
                }
                else if (jsonIndent == "")
                {
                    this.ShowMessageAsync("运行失败", "你还没有输入完参数呢!怎么跑");
                }
                else
                {
                    ExecuteCMD("bin/convert.exe", sourceDirPath + " -f " + sourceFormat + " --indent " + jsonIndent + " -r");
                    Run.IsEnabled = false;
                    Run.Content = "运行中...";
                }
            }
            else
            {
                if (sourceFormat == "")
                {
                    this.ShowMessageAsync("运行失败", "你还没有输入完参数呢!怎么跑");
                }
                else if (sourcePath == "")
                {
                    this.ShowMessageAsync("运行失败", "你还没有输入完参数呢!怎么跑");
                }
                else if (jsonIndent == "")
                {
                    this.ShowMessageAsync("运行失败", "你还没有输入完参数呢!怎么跑");
                }
                else
                {
                    ExecuteCMD("bin/convert.exe", sourcePath + " -f " + sourceFormat + " --indent " + jsonIndent);
                    Run.IsEnabled = false;
                    Run.Content = "运行中...";
                    deOutPutPath = sourcePath.Substring(0, sourcePath.Length - 3) + "vrc";
                }
            }
        }
        #endregion

        #region 选择源文件格式
        void radio_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        void radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn == null)
                return;
            if (btn.Name == "txtRadio")
            {
                sourceFormat = "txt";
            }
            if (btn.Name == "mlrcRadio")
            {
                sourceFormat = "mlrc";
            }
            if (btn.Name == "assRadio")
            {
                sourceFormat = "ass";
            }
        }
        #endregion

        private void README_Click(object sender, RoutedEventArgs e)
        {
            README readme = new README();
            readme.Show();
        }

        #region 选择文件 / 文件夹对话框
        private void SourcePathInput_Click(object sender, RoutedEventArgs e)
        {
            if (Recursion)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.Title = "请选择源目录";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    sourceDirPath = dialog.FileName;
                    SourceFileDisplay.Text = "源目录:" + sourceDirPath;
                }
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;//该值确定是否可以选择多个文件
                dialog.Title = "请选择源歌词文件";
                dialog.Filter = "所有文件(*.lrc,*.ass,*.txt)|*.lrc;*.ass;*.txt";
                if (dialog.ShowDialog() == null == false)
                {
                    sourcePath = dialog.FileName;
                    SourceFileDisplay.Text = "源文件位置:" + sourcePath;
                }
            }
        }

        private void OutPutPathInput_Click(object sender, RoutedEventArgs e)
        {
            if (Recursion)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.Title = "请选择输出目录";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    outputDirPath = dialog.FileName;
                    OutPutFileDisplay.Text = "输出目录:" + outputDirPath;
                }
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Vrc 文件(*.vrc)|*.vrc";
                if (dialog.ShowDialog() == null == false)
                {
                    outputPath = dialog.FileName;
                    OutPutFileDisplay.Text = "输出文件路径:" + outputPath;
                }
            }
        }
        #endregion

        #region 是否开启递归模式
        private void IsRecursion_Checked(object sender, RoutedEventArgs e)
        {
            SourceFileDisplay.Text = "源目录:" + sourceDirPath;
            SourcePathInput.Content = "选择源目录";
            if (outputDirPath == "")
            {
                OutPutFileDisplay.Text = "输出目录:源文件路径下";
            }
            else
            {
                OutPutFileDisplay.Text = "输出目录:" + outputDirPath;
            }
            OutPutPathInput.Content = "选择输出目录";
            Recursion = true;
        }

        private void IsRecursion_Unchecked(object sender, RoutedEventArgs e)
        {
            SourceFileDisplay.Text = "源文件路径:" + sourcePath;
            SourcePathInput.Content = "选择源文件路径";
            if (outputPath == "")
            {
                OutPutFileDisplay.Text = "输出目录:源文件路径下";
            }
            else
            {
                OutPutFileDisplay.Text = "输出目录:" + outputPath;
            }
            OutPutPathInput.Content = "选择输出文件路径";
            Recursion = false;
        }

        #endregion

        #region 执行命令
        private void ExecuteCMD(string StartFileName, string StartFileArg)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = StartFileName;
                process.StartInfo.Arguments = StartFileArg;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(ProcessExit);

                process.Start();
                // process.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                Run.Dispatcher.Invoke(new Action(delegate { Run.IsEnabled = true; Run.Content = "走你!"; }));
                Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("无法运行脚本", "错误信息:\r\n" + e.Message + "\r\n错误追踪:\r\n" + e.StackTrace); }));
                //OutPut.AppendText("无法运行脚本 错误信息:\r\n");
                //OutPut.AppendText(e.Message);
                //OutPut.AppendText("\r\n错误追踪:\r\n");
                //OutPut.AppendText(e.StackTrace+"\r\n");
            }
        }
        #endregion

        #region 输出日志
        private void WriteLog(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                OutPut.Dispatcher.Invoke(new Action(delegate { OutPut.AppendText(outLine.Data + "\r\n"); }));
            }
        }
        #endregion

        #region 进程退出事件
        private void ProcessExit(object sender, EventArgs e)
        {
            Run.Dispatcher.Invoke(new Action(delegate { Run.IsEnabled = true; Run.Content = "走你!"; }));
            if (Recursion)
            {
                #region 递归模式
                if (outputDirPath != "")
                {
                    #region 设定了输出目录
                    FileInfo[] files = FindAllFile(sourceDirPath, "*.vrc");
                    string sourceExt = sourceFormat;
                    if (sourceFormat == "mlrc")
                    {
                        sourceExt = "txt";
                    }
                    FileInfo[] sourceFiles = FindAllFile(sourceDirPath, "*." + sourceExt);
                    bool isOk = true;

                    if (files.Length == sourceFiles.Length)
                    {
                        #region 文件全部输出成功
                        for (int i = 0; i != files.Length; i++)
                        {
                            try
                            {
                                File.Move(files[i].FullName, outputDirPath + "/" + files[i].Name);
                            }
                            catch (Exception exx)
                            {
                                Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("无法将文件输出到指定目录", "文件将被输出到:" + files[i].FullName + "\r\n错误信息:\r\n" + exx.Message + "\r\n错误追踪:\r\n" + exx.StackTrace); }));
                                isOk = false;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 文件并未全部输出成功
                        isOk = false;
                        for (int i = 0; i != files.Length; i++)
                        {
                            try
                            {
                                File.Move(files[i].FullName, outputDirPath + "/" + files[i]);
                            }
                            catch (Exception exx)
                            {
                                Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出失败", "部分文件可能没有输出成功"); }));
                                isOk = false;
                            }
                        }
                        #endregion
                    }

                    if (isOk)
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出成功", "文件已输出到:" + outputDirPath); }));
                    } else
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出失败", "部分文件可能没有输出成功"); }));
                    }
                    #endregion
                }
                else
                {
                    #region 未设定输出目录
                    FileInfo[] files = FindAllFile(sourceDirPath, "*.vrc");
                    string sourceExt = sourceFormat;
                    if (sourceFormat == "mlrc")
                    {
                        sourceExt = "txt";
                    }
                    FileInfo[] sourceFiles = FindAllFile(sourceDirPath, "*." + sourceExt);
                    bool isOk = true;

                    if (files.Length != sourceFiles.Length)
                    {
                        isOk = false;
                    }

                    if (isOk)
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出成功", "文件已输出到源目录"); }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出失败", "部分文件可能没有输出成功"); }));
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 单文件模式
                if (outputPath == "")
                {
                    if (File.Exists(deOutPutPath))
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出成功", "文件已输出到:"+deOutPutPath); }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出失败", "请尝试手动调用脚本"); }));
                    }
                }
                else if (File.Exists(deOutPutPath))
                {
                    try
                    {
                        File.Move(deOutPutPath, outputPath);
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出成功", "文件已输出到:" + outputPath); }));
                    }
                    catch (Exception a)
                    {
                        Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("无法将文件输出到指定目录", "文件将被输出到:" + deOutPutPath + "\r\n错误信息:\r\n" + a.Message + "\r\n错误追踪:\r\n" + a.StackTrace); }));
                    }
                }
                else
                {
                    Dispatcher.Invoke(new Action(delegate { this.ShowMessageAsync("输出失败", "请尝试手动调用脚本"); }));
                }
                #endregion
            }
        }
        #endregion

        #region 寻找目录下包括子文件夹的文件
        private FileInfo[] FindAllFile(string path,string ext)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            return directoryInfo.GetFiles(ext, SearchOption.AllDirectories);
        }
        #endregion
    }
}
