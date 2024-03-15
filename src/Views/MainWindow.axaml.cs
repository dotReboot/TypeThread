using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using TypeThread.Models;

namespace TypeThread;

public partial class MainWindow : Window
{
    private readonly TextEditor textEditor;
    private readonly TextBlock fileNameTextBlock;
    private readonly TextBlock watermark;
    private readonly TextBlock timerText;
    private readonly Slider testTimeSlider;
    private readonly Grid testTimeSliderTicksValues;
    private readonly ProgressBar fileCompletionProgressBar;

    private bool isTestRunning;

    private TypingSample sample;
    private int currentPosition;
    private char CurrentCharacter => sample.Text[currentPosition];
    private int currentSecond;
    private readonly Timer timer;

    private int temporaryScore;
    private double[] results;

    private const double wordLength = 4.7;
    private const int timerInterval = 1000;
    private int testTime;

    public MainWindow()
    {
        InitializeComponent();
        textEditor = this.FindControl<TextEditor>("Editor");
        fileNameTextBlock = this.FindControl<TextBlock>("FileName");
        watermark = this.FindControl<TextBlock>("Watermark");
        timerText = this.FindControl<TextBlock>("TimerText");
        testTimeSlider = this.FindControl<Slider>("TestTimeSlider");
        testTimeSliderTicksValues = this.FindControl<Grid>("TestTimeSliderTicksValues");
        fileCompletionProgressBar = this.FindControl<ProgressBar>("FileCompletionProgressBar");
        timer = new Timer();
        timer.Interval = timerInterval;
        timer.Elapsed += OnTick;

        isTestRunning = false;

        var  _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var _textMateInstallation = textEditor.InstallTextMate(_registryOptions);
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));

        ConfigureKeyGestures();

        DisplayNextFile();
    }

    public void StartThread()
    {
        isTestRunning = true;
        testTime = (int) testTimeSlider.Value;
        results = new double[testTime];
        ToggleSettingsVisibility();
        StartTimer();
        timerText.Text = testTime.ToString();
    }

    public void StopThread()
    {
        isTestRunning = false;
        StopTimer();
        Dispatcher.UIThread.Post(() =>
        {
            DisplayNextFile();
            ToggleSettingsVisibility();
        });
    }

    private void DisplayNextFile()
    {
        ResetText();
        sample = FileManager.LoadNextFile();
        fileNameTextBlock.Text = sample.FileName;
        watermark.Text = sample.Text;
    }

    private void ResetText()
    {
        currentPosition = 0;
        fileCompletionProgressBar.Value = 0;
        textEditor.Text = String.Empty;
    }

    private void ToggleSettingsVisibility()
    {
        timerText.IsVisible = isTestRunning;
        fileCompletionProgressBar.IsVisible = isTestRunning;
        testTimeSlider.IsVisible = !isTestRunning;
        testTimeSliderTicksValues.IsVisible = !isTestRunning;
    }

    private void ShowResultWindowAsync()
    {
        int score = (int)Math.Round(results.Sum() / results.Length);
        Dispatcher.UIThread.Post(async () =>
        {
            var dialog = new ResultWindow(score);
            await dialog.ShowDialog(this);
        });
    }


    protected override void OnKeyDown(KeyEventArgs eventArgs)
    {
        switch (eventArgs.Key)
        {
            case Key.Escape:
                if (isTestRunning)
                {
                    StopThread();
                }
                else
                {
                    DisplayNextFile();
                }
                break;
            case Key.Enter:
                ProceedToNextLine();
                break;
            case Key.Tab:
                ProceedToNextWord();
                break;
            case Key.Left:
                if (!isTestRunning && testTimeSlider.Value != testTimeSlider.Minimum)
                {
                    testTimeSlider.Value -= testTimeSlider.TickFrequency;
                }
                break;
            case Key.Right:
                if (!isTestRunning && testTimeSlider.Value != testTimeSlider.Maximum)
                {
                    testTimeSlider.Value += testTimeSlider.TickFrequency;
                }
                break;
            default:
                var symbol = eventArgs.KeySymbol;
                if (symbol is null)
                {
                    return;
                }
                char keyCharacter = Convert.ToChar(symbol);
                if (CurrentCharacter == keyCharacter)
                {
                    if (!isTestRunning)
                    {
                        StartThread();
                    }
                    WriteToEditor(keyCharacter);
                    temporaryScore++;
                }
                break;
        }
    }

    private void ConfigureKeyGestures()
    {
        Dictionary<Key, char> numberKeyGestures = new Dictionary<Key, char>{ {Key.D1, '!'}, {Key.D2, '@'},
                                                            {Key.D3, '#'},{Key.D4, '$'},
                                                            {Key.D5, '%'}, {Key.D6, '^'},
                                                            {Key.D7, '&'}, {Key.D8, '*'},
                                                            {Key.D9, '('}, {Key.D0, ')'} };
        foreach (Key key in numberKeyGestures.Keys)
        {
            char keyCharacter = numberKeyGestures[key];
            this.KeyDown += (sender, e) =>
            {
                if (new KeyGesture(key, KeyModifiers.Shift).Matches(e) && (CurrentCharacter == keyCharacter))
                {
                    WriteToEditor(keyCharacter);
                }
            };
        }
    }


    private void WriteToEditor(char characterToWrite)
    {
        textEditor.Text += characterToWrite;
        if (sample.Text.Length - 1 == currentPosition)
        {
            DisplayNextFile();
            return;
        }
        currentPosition++;
        fileCompletionProgressBar.Value = (double)currentPosition/watermark.Text.Length;
    }

    private void ProceedToNextLine(bool isAutoIndentEnabled = true)
    {
        if (CurrentCharacter == '\r')
        {
            WriteToEditor('\r');
        }
        if (CurrentCharacter == '\n')
        {
            WriteToEditor('\n');
            temporaryScore++;
        }
        if (isAutoIndentEnabled)
        {
            ProceedToNextWord();
        }
    }

    private void ProceedToNextWord()
    {
        while (CurrentCharacter == ' ')
        {
            WriteToEditor(' ');
        }
    }


    private void StartTimer() => timer.Enabled = true;

    private void StopTimer()
    {
        timer.Enabled = false;
        currentSecond = 0;
    }

    private void OnTick(object source, ElapsedEventArgs args)
    {
        WriteTemporaryScore();
        currentSecond++;
        Dispatcher.UIThread.Post(() => timerText.Text = (testTime - currentSecond).ToString());
        if (currentSecond == testTime)
        {
            StopThread();
            ShowResultWindowAsync();
        }
    }


    private void WriteTemporaryScore()
    {
        results[currentSecond] = (double)(temporaryScore / wordLength * testTime);
        temporaryScore = 0;
    }
}