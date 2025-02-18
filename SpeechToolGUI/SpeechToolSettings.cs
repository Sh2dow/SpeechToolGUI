using System.IO;
using System.Text.Json;

namespace SpeechToolGUI;

public class SpeechToolSettings
{
    public string IdxFilePath { get; set; }
    public string BigFilePath { get; set; }
    public string ExportPath { get; set; }
    public TargetConsoleEnum TargetConsole { get; set; } // New property to store the selected console
    public TargetGameEnum TargetGame { get; set; } // New property to store the selected console

    public SpeechToolSettings(string idxFilePath, string bigFilePath, string exportPath, TargetConsoleEnum targetConsole, TargetGameEnum targetGame)
    {
        IdxFilePath = idxFilePath;
        BigFilePath = bigFilePath;
        ExportPath = exportPath;
        TargetConsole = targetConsole;
        TargetGame = targetGame;
    }

    public SpeechToolSettings()
    {
        IdxFilePath = string.Empty;
        BigFilePath = string.Empty;
        ExportPath = string.Empty;
        TargetConsole = TargetConsoleEnum.PC;
        TargetGame = TargetGameEnum.MostWanted;
    }

    public void Save(string filePath)
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public static SpeechToolSettings? Load(string filePath)
    {
        if (!File.Exists(filePath))
            return new SpeechToolSettings();
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<SpeechToolSettings>(json);
    }
}