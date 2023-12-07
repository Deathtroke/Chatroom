using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class UserInfo
{
	public string Name;
	public int Id;
}
public partial class connection_handler : Node2D
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

	private text_handler text_handler;

	private bool connecting;
	
	private bool headless;
	
	
	
	private static List<UserInfo> user = new List<UserInfo>();
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var arguments = new Godot.Collections.Dictionary();
		foreach (var argument in OS.GetCmdlineArgs())
		{
			if (argument.Find("=") > -1)
			{
				string[] keyValue = argument.Split("=");
				arguments[keyValue[0].Replace("--","")] = keyValue[1];
			}
			else
			{
				// Options without an argument will be present in the dictionary,
				// with the value set to an empty string.
				string[] keyValue = argument.Split("=");
				arguments[keyValue[0].Replace("--","")] = "";
			}
		}
		GD.Print(arguments);
		
		Multiplayer.PeerDisconnected += peer_disconnected;
		Multiplayer.ConnectedToServer += connected_to_server;
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

		connecting = false;
		host_pressed();

		headless = false;
		
		GD.Print(DisplayServer.GetName());
		if (DisplayServer.GetName() == "headless")
		{
			NameText.Text = "server";
			portText.Text = "1234";
			headless = true;
			confirm_pressed();
		}
	}

	//enables the Input fields that are needed to connect 
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
	
	//enables the Input fields that are needed to host 
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
		
		text_handler = GetNode<text_handler>("../Chat");
	}

	// creates the client the the provided data and tries to connect to the server
	void connect_to_Server(String address, int port)
	{
		connecting = true;
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

	// creates the server the the provided data
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

	private void peer_disconnected(long id)
	{
		GD.Print($"{id} disconnected");
	}
	
	private void ConnectionFailed()
	{
		GD.Print("Connection failed");
	}

	//Multiplayer event for when connection successfully established. Sends own client data to server
	private void connected_to_server()
	{
		GD.Print("connected to server");
		RpcId(1, "sendPlayerInformation", username, Multiplayer.GetUniqueId());
	}

	//handles the input message, check it for commands and send the message 
	public void send_message(String message)
	{
		if (message.ToCharArray()[0] == '/')
		{
			chat_commands(message);
			return;
		}
		Rpc("receive_message", username, message);
	}

	//parses and executes the commands in chat
	private void chat_commands(string command)
	{
		switch (command)
		{
			case "/list":
				String list_persons = user.Aggregate("", (current, person) => current + (" " + person.Name + " "));
				text_handler.update_text(list_persons);
				break;
			default:
				break;
		}
	}
	
	//multiplayer listener 
	//listens for any send chat message
	[Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void receive_message(String name, String message)
	{
		GD.Print($"received {message} from {name}");
		if (!headless)
		{
			text_handler.update_text(name, message);
		}
	}

	//multiplayer listener 
	//Adds client to the user-list, if instance is server it broadcast that newly added client 
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void sendPlayerInformation(string name, int id){
		UserInfo playerInfo = new UserInfo(){
			Name = name,
			Id = id
		};

		bool new_user = true;
		foreach (var u in user)
		{
			if (u.Id == playerInfo.Id)
			{
				new_user = false;
			}
		}
		
		if(new_user){
			user.Add(playerInfo);
			if (connecting)
			{
				if (name == username)
				{
					// builds String with all current User
					string already_joined = user.Where(u => u.Name != username).Aggregate("", (current, u) => current + (u.Name + ", "));
					already_joined = already_joined.Remove(already_joined.Length - 2);
					
					text_handler.update_text("currently in chat: " + already_joined);
					
					text_handler.update_text(name + " joined");
				
					//complete connection phase
					connecting = false;
				}
			}
			else
			{
				if (!headless)
				{
					text_handler.update_text(name + " joined");
				}
			}
		}

		//check if instance is host. if so it sends the new client data to all clients so they also can update their list
		if (!Multiplayer.IsServer()) return;
		foreach (var item in user)
		{
			Rpc("sendPlayerInformation", item.Name, item.Id);
		}
	}
}
