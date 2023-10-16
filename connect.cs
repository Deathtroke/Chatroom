using Godot;
using System;

public partial class connect : Node2D
{
	private Button btnConnect;
	private Button btnHost;
	private Button btnConfirm;

	private TextEdit ipText;
	private TextEdit portText;
	private TextEdit name;

	private bool host;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		btnConnect = GetNode<Button>("Connect");
		btnHost = GetNode<Button>("Host");
		btnConfirm = GetNode<Button>("Confirm");
		
		btnConnect.Pressed += ConnectPressed;
		btnHost.Pressed += HostPressed;
		btnConfirm.Pressed += ConfirmPressed;

		ipText = GetNode<TextEdit>("IP");
		portText = GetNode<TextEdit>("Port");
		name = GetNode<TextEdit>("Name");
	}

	private void ConnectPressed()
	{
		btnConfirm.Text = "Connect";
		ipText.Visible = true;
		host = false;
	}
	
	private void HostPressed()
	{
		btnConfirm.Text = "Host";
		ipText.Visible = false;
		host = true;
	}

	private void ConfirmPressed()
	{
		var ip = ipText.Text;
		var port = portText.Text;
		var username = name.Text;

		if (ip == "" || port == "" || username == "")
		{
			
			return;
		}
		if (host)
		{
			host_server();
		}
		else
		{
			connect_to_Server();
		}
	}

	void connect_to_Server()
	{
		
	}

	void host_server()
	{
		
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
