using Godot;
using System;

public partial class ButtonHandler : Button
{
	private RichTextLabel text;
	private TextEdit input;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Text = "Click me";
		this.Pressed += ButtonPressed;

		
		text = GetNode<RichTextLabel>("../RichTextLabel");
		input = GetNode<TextEdit>("../TextEdit");
	}
	
	private void ButtonPressed()
	{
		GD.Print("Hello world!");
		text.Text += input.Text + "\n";
		input.Text = "";
	}	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
//	public override void _Process(double delta)
//	{
//	}
}
