using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWeatherApp.Plugins;

public class ArchivePlugin
{
    [KernelFunction("write_data")]
    [Description("Saves data to a specified file in the history directory.")]
    public async Task WriteData(Kernel kernel, string fileName, string data)
    {
        var historyDirectory = "../../../Archive";
        var filePath = Path.Combine(historyDirectory, fileName);
    
        // Ensure the directory exists
        Directory.CreateDirectory(historyDirectory);
        await File.WriteAllTextAsync(filePath, data);
    }
}