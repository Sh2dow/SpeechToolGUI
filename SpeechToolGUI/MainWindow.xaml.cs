using System.Windows;
using System.IO;
using Microsoft.Win32;
using SpeechTool;

namespace SpeechToolGUI;

public partial class MainWindow : Window
{
    private const string SettingsFileName = "SpeechToolSettings.json";

    // Member variables in your MainWindow class
    private string _currentIdxPath;
    private string _currentBigPath;
    private string _currentExportPath;
    private TargetConsoleEnum _currentConsole;
    private TargetGameEnum _currentGame;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenProject_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "IDX Files (*.idx;*.IDX)|*.idx;*.IDX",
            Title = "Open IDX Project"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            _currentIdxPath = openFileDialog.FileName;
            string extension = Path.GetExtension(_currentIdxPath);
            string baseName = Path.GetFileNameWithoutExtension(_currentIdxPath);
            string? directory = Path.GetDirectoryName(_currentIdxPath);
            _currentBigPath = Path.Combine(directory, baseName + ".big");

            // Spawn ParamsSelectionWindow to let the user choose the target console and game.
            var paramsWindow = new ParamsSelectionWindow { Owner = this };
            if (paramsWindow.ShowDialog() != true)
            {
                // User cancelled console/game selection; abort further processing.
                return;
            }

            // Acquire the selected enum values.
            _currentConsole = paramsWindow.SelectedConsole;
            _currentGame = paramsWindow.SelectedGame;

            // Save project paths (including target console and game) for later use.
            var settings = new SpeechToolSettings
            {
                IdxFilePath = _currentIdxPath,
                BigFilePath = _currentBigPath,
                ExportPath = Path.Combine(directory, "Exported"),
                TargetConsole = _currentConsole,
                TargetGame = _currentGame
            };
            string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);
            settings.Save(settingsPath);

            // Uppercase extension means export mode.
            if (extension.Equals(".IDX", StringComparison.Ordinal))
            {
                if (File.Exists(_currentBigPath))
                {
                    // Use your existing Extraction logic to extract audio samples.
                    Extraction.ExtractSamples(_currentBigPath);
                    MessageBox.Show("Audio samples extracted from the BIG file.", "Export Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("BIG file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Lowercase extension: load project for editing/compiling.
            {
                // Use your existing IDXFile logic to load the IDX project.
                IDXFile.ReadIDX(_currentIdxPath);
                MessageBox.Show("Project loaded. You can now replace audio and compile the project.", "Project Loaded",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Helper method to save project paths for later use
    private void SaveProjectPaths()
    {
        if (string.IsNullOrEmpty(_currentIdxPath) || string.IsNullOrEmpty(_currentBigPath))
        {
            MessageBox.Show("No project is loaded.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Ensure the export path is set (for example, as a subfolder in the IDX file's directory)
        _currentExportPath = Path.Combine(Path.GetDirectoryName(_currentIdxPath) ?? string.Empty, "Exported");

        var settings = new SpeechToolSettings
        {
            IdxFilePath = _currentIdxPath,
            BigFilePath = _currentBigPath,
            ExportPath = _currentExportPath,
            TargetConsole = _currentConsole,
            TargetGame = _currentGame
        };

        string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);
        settings.Save(settingsPath);

        MessageBox.Show("Project paths saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
    }

// Event handler for the "Save" menu item
    private void SaveProject_Click(object sender, RoutedEventArgs e)
    {
        SaveProjectPaths();
    }

    private void ImportAudio_Click(object sender, RoutedEventArgs e)
    {
        // Call a method from SpeechTool to import audio files
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        // Call a method from SpeechTool to export files
    }

    private void CompileFiles_Click(object sender, RoutedEventArgs e)
    {
        string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);
        var settings = SpeechToolSettings.Load(settingsPath);

        if (settings != null && File.Exists(settings.IdxFilePath))
        {
            // Assume new or replaced ASF files are in a folder named "Edited" inside the BIG file directory.
            string asfFolder = Path.Combine(Path.GetDirectoryName(settings.BigFilePath) ?? string.Empty, "Edited");

            // Verify that the "Edited" folder exists.
            if (!Directory.Exists(asfFolder))
            {
                MessageBox.Show(
                    "The 'Edited' folder does not exist. Please import or replace audio files before compiling.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verify that there are ASF files in the "Edited" folder.
            var asfFiles = Directory.GetFiles(asfFolder, "*.ASF");
            if (asfFiles.Length == 0)
            {
                MessageBox.Show(
                    "No ASF files found in the 'Edited' folder. Please import or replace audio files before compiling.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Recompile the BIG file using the new ASF files.
            Extraction.RecompileBigFile(asfFolder, settings.BigFilePath);
            MessageBox.Show("BIG file recompiled successfully.", "Compilation Successful", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Project settings or files are missing.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        // Open a settings window or dialog
    }

    private void DragArea_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private void DragArea_Drop(object sender, DragEventArgs e)
    {
        // Handle file drop and import using SpeechTool functionality
    }
}