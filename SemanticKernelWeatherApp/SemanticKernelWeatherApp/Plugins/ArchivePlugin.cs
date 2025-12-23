using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWeatherApp.Plugins;

public class ArchivePlugin
{
    [KernelFunction("write_data")]
    [Description("Saves data to a specified file in the history directory.")]
    public async Task WriteData(Kernel kernel, string fileName, string data)
    {
        await File.WriteAllTextAsync($"./history/{fileName}", data);
    }
}