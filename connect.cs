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

	private ENetMultiplayerPeer peer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		
		btnConnect = GetNode<Button>("Connect");
		btnHost = GetNode<Button>("Host");
		btnConfirm = GetNode<Button>("Confirm");
		
		btnConnect.Pressed += ConnectPressed;
		btnHost.Pressed += HostPressed;
		btnConfirm.Pressed += ConfirmPressed;

		ipText = GetNode<TextEdit>("IP");
		portText = GetNode<TextEdit>("Group/Port");
		name = GetNode<TextEdit>("Group/Name");
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

		if (port == "" || username == "")
		{
			if (username == "")
			{
				GD.Print("username missing");
			}
			if (port == "")
			{
				GD.Print("port missing");
			}
			return;
		}
		if (host)
		{
			host_server(port.ToInt());
		}
		else
		{
			if (ip == "")
			{
				GD.Print("ip missing");
				return;
			}
			connect_to_Server(ip, port.ToInt());
		}

		var scene = ResourceLoader.Load<PackedScene>("res://chat.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	void connect_to_Server(String address, int port)
	{
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateClient(address, port);
		if (error != Error.Ok)
		{
			GD.Print(error);
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("connecting to server");
	}

	void host_server(int port)
	{
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateServer(port);
		if (error != Error.Ok)
		{
			GD.Print(error);
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("server started");
	}

	private void PeerConnected(long ID)
	{
		GD.Print($"{ID} connected");
	}

	private void PeerDisconnected(long ID)
	{
		GD.Print($"{ID} disconnected");
	}
	
	private void ConnectionFailed()
	{
		GD.Print("Connection failed");
	}

	private void ConnectedToServer()
	{
		GD.Print("connected to server");
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
