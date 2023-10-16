using Godot;
using System;

public partial class connect : Node2D
{
	private Button btnConnect;
	private Button btnHost;
	private Button btnConfirm;

	private TextEdit ip;
	private TextEdit port;
	private TextEdit name;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		btnConnect = GetNode<Button>("Connect");
		btnHost = GetNode<Button>("Host");
		btnConfirm = GetNode<Button>("Confirm");
		
		btnConnect.Pressed += ConnectPressed;
		btnHost.Pressed += HostPressed;
		btnConfirm.Pressed += ConfirmPressed;

		ip = GetNode<TextEdit>("IP");
		port = GetNode<TextEdit>("Port");
		name = GetNode<TextEdit>("Name");
	}

	private void ConnectPressed()
	{
		btnConfirm.Text = "Connect";
		ip.Visible = true;
	}
	
	private void HostPressed()
	{
		btnConfirm.Text = "Host";
		ip.Visible = false;
	}

	private void ConfirmPressed()
	{
		
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
