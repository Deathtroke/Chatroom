using Godot;
using System;

public partial class ButtonHandler : Node2D
{
	private RichTextLabel text;
	private TextEdit input;
	private connect connection_handler;
	private Button button;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button = GetNode<Button>("Button");
		button.Text = "Send Message";
		button.Pressed += ButtonPressed;

		
		text = GetNode<RichTextLabel>("RichTextLabel");
		input = GetNode<TextEdit>("TextEdit");
		connection_handler = GetNode<connect>("../Connect");
	}
	
	private void ButtonPressed()
	{
		//send_messange(input.Text);
		connection_handler.send_message(input.Text);
		input.Text = "";
	}

	public void update_text(String name, String message)
	{
		text.Text += $"[{name}]: {message}\n";
	}
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
//	public override void _Process(double delta)
//	{
//	}
}
