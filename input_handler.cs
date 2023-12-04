using Godot;
using System;

public partial class input_handler : TextEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// binds the on_text_change function the the TextChange event of the TextEdit field this script is attached to
		TextChanged += on_text_change;
	}

	//Handle Tab input to switch to next Element
	private void on_text_change()
	{
		//checks and replaces tab
		if (Text.Contains("\t"))
		{
			Text = Text.Replace("\t", "");
			focus_next();
		}
		//checks and replaces line break
		else if (Text.Contains("\n"))
		{
			Text = Text.Replace("\n", "");
			focus_next();
		}
	}

	//Checks if the next focus object is a Button or Input field and sets the focus on that object
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
}
