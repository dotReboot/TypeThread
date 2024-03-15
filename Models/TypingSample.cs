namespace TypeThread.Models;

public class TypingSample
{
    public string FileName;
    public string Text;

    public TypingSample(string fileName, string text)
    {
        FileName = fileName;
        Text = text;
    }
}