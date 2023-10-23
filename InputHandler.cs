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
			if (GetNode(FocusNext).GetType() == typeof(Button))
			{
				GetNode<Button>(FocusNext).GrabFocus();
			}
			else
			{
				if (GetNode<TextEdit>(FocusNext).Visible)
				{
					GetNode<TextEdit>(FocusNext).GrabFocus();	
				}
				else
				{
					GetNode<TextEdit>(FocusNext).Text = "\t";
				}
				
			}
			
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
