using Godot;
using System;

public partial class InputHandler : TextEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//this.Text = "Send Message";
		TextChanged += OnTextChanged;
	}

	//Handle Tab input to switch to next Element
	private void OnTextChanged()
	{
		if (Text.Contains("\t"))
		{
			Text = Text.Replace("\t", "");
			focus_next();
		}
		else if (Text.Contains("\n"))
		{
			Text = Text.Replace("\n", "");
			focus_next();
		}
	}

	private void focus_next()
	{
		if (GetNode(FocusNext).GetType() == typeof(Button))
		{
			GetNode<Button>(FocusNext).GrabFocus();
		}
		else
		{
			GetNode<TextEdit>(FocusNext).GrabFocus();	
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
