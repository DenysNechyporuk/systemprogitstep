using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace FileCopyHW;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void StartButtonClick(object sender, RoutedEventArgs e)
    {
        string source = srcTextBox.Text;
        string destination = destTextBox.Text;

        if (!Directory.Exists(source))
        {
            MessageBox.Show("Source directory not found");
            return;
        }
        if (destination == "")
        {
            MessageBox.Show("Enter destination directory");
            return;
        }

        string sourcePath = Path.GetFullPath(source).TrimEnd('\\');
        string targetPath = Path.Combine(Path.GetFullPath(destination), new DirectoryInfo(sourcePath).Name);

        if (targetPath.StartsWith(sourcePath + "\\", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Can not copy directory into itself");
            return;
        }

        startButton.IsEnabled = false;
        progress.Value = 0;
        percentText.Text = "0 %";

        Progress<int> progressReporter = new Progress<int>(value =>
        {
            progress.Value = value;
            percentText.Text = value + " %";
        });

        try
        {
            await CopyDirectoryAsync(sourcePath, targetPath, progressReporter);
            MessageBox.Show("Complete");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        startButton.IsEnabled = true;
    }

    private Task CopyDirectoryAsync(string source, string target, IProgress<int> reporter)
    {
        return Task.Run(() =>
        {
            string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            string[] directories = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);

            long totalBytes = 0;
            foreach (string file in files)
            {
                totalBytes += new FileInfo(file).Length;
            }

            Directory.CreateDirectory(target);
            foreach (string directory in directories)
            {
                Directory.CreateDirectory(Path.Combine(target, Path.GetRelativePath(source, directory)));
            }

            long copiedBytes = 0;
            int lastPercent = 0;
            byte[] buffer = new byte[1024 * 64];

            foreach (string file in files)
            {
                string destFile = Path.Combine(target, Path.GetRelativePath(source, file));
                using (FileStream srcStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                    {
                        int bytes;
                        while ((bytes = srcStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destStream.Write(buffer, 0, bytes);
                            copiedBytes += bytes;

                            int percent = (int)(copiedBytes * 100 / totalBytes);
                            if (percent != lastPercent)
                            {
                                lastPercent = percent;
                                reporter.Report(percent);
                            }
                        }
                    }
                }
            }

            reporter.Report(100);
        });
    }

    private void OpenSourceButtonClick(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog dialog = new OpenFolderDialog();
        if (dialog.ShowDialog() == true)
        {
            srcTextBox.Text = dialog.FolderName;
        }
    }

    private void OpenDestButtonClick(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog dialog = new OpenFolderDialog();
        if (dialog.ShowDialog() == true)
        {
            destTextBox.Text = dialog.FolderName;
        }
    }
}