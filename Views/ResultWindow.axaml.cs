using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace TypeThread;

public partial class ResultWindow : Window
{
	private readonly TextBlock resultText;

	public ResultWindow(int score)
	{
		InitializeComponent();
		resultText = this.FindControl<TextBlock>("ResultText");
		resultText.Text = $"Your code typing speed is {score} wpm";
	}

	protected override void OnKeyDown(KeyEventArgs eventArgs)
	{
		switch (eventArgs.Key)
		{
			case Key.Escape:
			case Key.Enter:
			case Key.Space:
				this.Close();
				break;
		}
	}

	public void CloseWindow(object sender, RoutedEventArgs args) => this.Close();
}