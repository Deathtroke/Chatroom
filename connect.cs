using Godot;
using System;
using System.Collections.Generic;

public class UserInfo
{
	public string Name;
	public int Id;
}
public partial class connect : Node2D
{
	private Button btnConnect;
	private Button btnHost;
	private Button btnConfirm;

	private TextEdit ipText;
	private TextEdit portText;
	private TextEdit NameText;

	private bool host;

	private String username;

	private ENetMultiplayerPeer peer;

	private ButtonHandler text_handler;
	
	
	
	private static List<UserInfo> user = new List<UserInfo>();
	
	
	
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
		btnHost.GrabFocus();
		
		
		btnConnect.Pressed += connect_pressed;
		btnHost.Pressed += host_pressed;
		btnConfirm.Pressed += confirm_pressed;

		ipText = GetNode<TextEdit>("IP");
		portText = GetNode<TextEdit>("Group/Port");
		NameText = GetNode<TextEdit>("Group/Name");
	}

	private void connect_pressed()
	{
		btnConfirm.Text = "Connect";
		ipText.Visible = true;
		host = false;
		portText.FocusNext = ipText.GetPath();
		
		//for testing
		NameText.Text = "client";
		portText.Text = "1234";
		ipText.Text = "127.0.0.1";
	}
	
	private void host_pressed()
	{
		btnConfirm.Text = "Host";
		ipText.Visible = false;
		host = true;
		portText.FocusNext = btnConfirm.GetPath();
		ipText.Text = "";
		
		//for testing
		NameText.Text = "server";
		portText.Text = "1234";
	}

	private void confirm_pressed()
	{
		var ip = ipText.Text;
		var port = portText.Text;
		username = NameText.Text;

		//test for valid data
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
		
		//process data
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
		
		//switch scene
		var scene = ResourceLoader.Load<PackedScene>("res://chat.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
		
		text_handler = GetNode<ButtonHandler>("../Chat");
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
		user.Add(new UserInfo(){
			Name = username,
			Id = 1
		});
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	void get_username(String name){
		GD.Print($"{name} joined");
		text_handler.update_text(name + " joined");
	}

	private void PeerConnected(long id)
	{
		GD.Print($"{id} connected");
		Rpc("get_username",username);
	}

	private void PeerDisconnected(long id)
	{
		GD.Print($"{id} disconnected");
	}
	
	private void ConnectionFailed()
	{
		GD.Print("Connection failed");
	}

	private void ConnectedToServer()
	{
		GD.Print("connected to server");
		RpcId(1, "sendPlayerInformation", username, Multiplayer.GetUniqueId());
	}

	public void send_message(String message)
	{
		if (message.ToCharArray()[0] == '/')
		{
			if (message == "/list")
			{
				String list_persons = "";
				foreach (UserInfo person in user)
				{
					list_persons += " " + person.Name + " ";
				}
				text_handler.update_text(list_persons);
			}
			return;
		}
		Rpc("receive_message", username, message);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void receive_message(String name, String message)
	{
		GD.Print($"received {message} from {name}");
		text_handler.update_text(name, message);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void sendPlayerInformation(string name, int id){
		UserInfo playerInfo = new UserInfo(){
			Name = name,
			Id = id
		};
		
		if(!user.Contains(playerInfo)){
			
			user.Add(playerInfo);
			
		}

		if(Multiplayer.IsServer()){
			foreach (var item in user)
			{
				Rpc("sendPlayerInformation", item.Name, item.Id);
			}
		}
	}
}
