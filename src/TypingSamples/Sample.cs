using System;

public class Sample
{
    public string Word {get; set;}
    public void Write()
    {
        foreach (char character in Word)
        {
            Console.WriteLine(character);
        }
    }
}