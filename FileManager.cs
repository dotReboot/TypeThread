using System;
using System.IO;
using TypeThread.Models;

namespace TypeThread;

public static class FileManager
{
    private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TypingSamples");
    private static Random random = new Random();
    private static DirectoryInfo directoryInfo = new DirectoryInfo(path);

    public static TypingSample LoadNextFile()
    {
        string fileName = GetTypingSampleFileName();
        string filePath = Path.Combine(path, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }
        string text = File.ReadAllText(filePath);
        TypingSample sample = new TypingSample(fileName, text);
        return sample;
    }

    private static string GetTypingSampleFileName()
    {
        FileInfo[] files = directoryInfo.GetFiles();
        return files[random.Next(files.Length)].Name;
    }
}
