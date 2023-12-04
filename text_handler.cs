using Godot;
using System;

public partial class text_handler : Node2D
{
	private RichTextLabel text;
	private TextEdit input;
	private connection_handler connection_handler;
	private Button send_message_button;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//gets the "Send Message"-Button and binds it Pressed event
		send_message_button = GetNode<Button>("Button");
		send_message_button.Pressed += send_message_pressed;

		//gets the text objects from the current scene
		text = GetNode<RichTextLabel>("RichTextLabel");
		input = GetNode<TextEdit>("TextEdit");
		
		//gets the current connection_handler instance
		connection_handler = GetNode<connection_handler>("../Connect");
	}
	
	//sends the input text to the connection_handler to broadcast it and clears the input 
	private void send_message_pressed()
	{
		connection_handler.send_message(input.Text);
		input.Text = "";
	}

	//adds a new line to the displayed chat history when a message arrives
	//overloaded to manage messages that do and dont have a sender
	public void update_text(String name, String message)
	{
		text.Text += $"[{name}]: {message}\n";
	}
	
	public void update_text(String message)
	{
		text.Text += $"{message}\n";
	}
}
