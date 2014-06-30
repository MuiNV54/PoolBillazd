using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using UnityEngine;
using System.Collections;

using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>

/// </summary>
public class NetworkClient : PhotonClient
{
	public string RoomName;
	public string LobbyName;
	public string UserName = String.Empty;
	/// The events data sent via networks
	public bool isNextPlayer;
	public Vector3 ForceToBall = Vector3.zero;
	public float CueAngle = 0.0f;
	public Vector3 CuePos = Vector3.zero;
	public int turnStyle = 0;

	/// States of the application
	public enum NetworkStateOption : byte
	{
		/// <summary>Used on start and on disconnect.</summary>
		Offline,
		/// <summary>Used on connect and when a room is left or on disconnect.</summary>
		OutsideOfRooms,
		/// <summary>Used as transition from OutsideOfRooms to InLobbyRoom. Checked when the result for OpJoin is handled. Has no GUI.</summary>
		JoiningLobbyRoom,
		/// <summary>Used in the lobby room.</summary>
		InLobbyRoom,
		/// <summary>Used as transition to InRoom. Checked when the result for OpJoin is handled. Has no GUI.</summary>
		JoiningRoom,
		/// <summary>Used while in a room.</summary>
		InRoom
	}
	/// Current state of the application
	public NetworkStateOption AppState = NetworkStateOption.Offline;
	/// Caches a lobby's room info
	public Hashtable RoomHashtable = new Hashtable();
	/// A cache of Actor/User properties in room
	public Hashtable ActorProperties = new Hashtable();
	/// Caches the list of players in a room. To make this more clean, it should be reset on leave/disconnect.</summary>
	public int[] ActorNumbersInRoom = new int[0];
	/// Custom events to be sent over network
	public enum GameEventCode : byte 
	{
		nextPlayer = 1,
		ForceApplied = 4,
		CueRotation = 9,
		CueDragging = 16, 
		StyleNoti = 25
	}
	public enum GameEventKey : byte
	{
		boolNext = 2,
		force = 3,
		rotation = 5,
		dragging = 8,
		style = 13
	}
	/// This demo supports only Name but could be extended with more properties easily.</summary>
	public enum NetworkActorProperties : byte { Name = (byte)'n' }

	/// <summary>
	/// On this level, only the connect and disconnect status callbacks are interesting and used.
	/// But we call the base-classes callback as well, which handles states in more detail.
	/// </summary>
	public override void OnStatusChanged(StatusCode statusCode)
	{
		base.OnStatusChanged(statusCode);
		
		switch (statusCode)
		{
		case StatusCode.Connect:
			AppState = NetworkStateOption.OutsideOfRooms;
			break;
		case StatusCode.Disconnect:
			AppState = NetworkStateOption.Offline;
			break;
		default:
			DebugReturn("StatusCode not handled: " + statusCode);
			break;
		}
	}

	/// <summary>
	/// For this client, we change states after joining a lobby or room was successfuly done. 
	/// Also, properties of other users in a room (fetched with GetProperties) are cached.
	/// </summary>
	public override void OnOperationResponse(OperationResponse operationResponse)
	{
		base.OnOperationResponse(operationResponse);

		switch(operationResponse.OperationCode)
		{
		case (byte)LiteOpCode.Join:
			if(this.AppState == NetworkStateOption.JoiningRoom)
			{
				// onJoining a room, get every properties of other players in room
				this.Peer.OpGetProperties(0);
				this.AppState = NetworkStateOption.InRoom;
			}
			if(this.AppState == NetworkStateOption.JoiningLobbyRoom)
				this.AppState = NetworkStateOption.InLobbyRoom;
			break;
		case (byte)LiteOpCode.GetProperties:
			// the result of operation GetProperties contains two interesting hashtables: one for actors and one for the room properties
			// this demo uses a property per actor for the name, so we are caching the complete list we just got
			Hashtable actorProps = operationResponse[(byte)LiteOpKey.ActorProperties] as Hashtable;
			this.ActorProperties = actorProps;  // updates (name changes, new users) are sent by the server as event and merged in to this
			break;
		}

	}

	/// <summary>
	/// Most of the "work" in the chat demo is done by events. 
	/// The events Join, Leave, RoomList and RoomListUpdate are the ones pre-defined by LiteLobby. 
	/// The Message event is a "custom event" that is defined, sent and used only by the Chat Demo.
	/// </summary>
	public override void OnEvent(EventData photonEvent)
	{
		base.OnEvent(photonEvent);

		// Custom events (defined and sent by a client) encapsulate the sent data in a separate Hashtable. This avoids duplicate usage of keys.
		// The event content a client sends is under key Data. This demo mostly handles custom data, so let's "grab" our content for later use
		Hashtable evData = photonEvent[(byte)LiteEventKey.Data] as Hashtable;
		int originatingActorNr = 0;
		if (photonEvent.Parameters.ContainsKey((byte)LiteEventKey.ActorNr))
		{
			originatingActorNr = (int)photonEvent[(byte)LiteEventKey.ActorNr];
		}

		switch(photonEvent.Code)
		{
			// this client or any other joined the room (lobbies will not send this event but rooms do)
		case (byte)LiteEventCode.Join:
			// update the list of actor numbers in room
			this.ActorNumbersInRoom = (int[])photonEvent[(byte)LiteOpKey.ActorList];
			
			// update the list of actorProperties if any were set on join
			Hashtable actorProps = photonEvent[(byte)LiteEventKey.ActorProperties] as Hashtable;
			if (actorProps != null)
			{
				this.ActorProperties[originatingActorNr] = actorProps;
			}
			break;
			
			// some other user left this room - remove his data
		case (byte)LiteEventCode.Leave:
			// update the list of actor numbers in room
			this.ActorNumbersInRoom = (int[])photonEvent[(byte)LiteOpKey.ActorList];
			
			// update the list of actorProperties we cache
			if (this.ActorProperties.ContainsKey(originatingActorNr))
			{
				this.ActorProperties.Remove(originatingActorNr);
			}
			break;
		case (byte)LiteLobbyPeer.LiteLobbyEventCode.RoomList:
		case (byte)LiteLobbyPeer.LiteLobbyEventCode.RoomListUpdate:
			// these two events are sent by lobby rooms
			// the first is a complete list (provided on join) and the latter an update to the list (when some room's user count changed)
			Hashtable roomData;
			roomData = (Hashtable)photonEvent[(byte)LiteLobbyPeer.LiteLobbyEventKey.RoomsArray];
			
			// updates exclude rooms in which the user count is not changed
			// so we merge and update new data into a local cache
			foreach (string key in roomData.Keys)
			{
				//each key is a room name. each value of a key is the current player count
				//we still list rooms when they are known and have 0 players. this could be changed easily
				this.RoomHashtable[key] = roomData[key];
			}
			break;
		/// Process the ForceApplied to the balls
		case (byte)GameEventCode.ForceApplied:
			// Store the received ForceToBall
			ForceToBall = (Vector3)evData[(byte)GameEventKey.force];
			break;
		/// Process the DraggingCue event
		case (byte)GameEventCode.CueDragging:
			// Store the received CuePos
			CuePos = (Vector3)evData[(byte)GameEventKey.dragging];
			break;
		/// Process the CueRotation event
		case (byte)GameEventCode.CueRotation:
			// Store the received CueAngle
			CueAngle = (float)evData[(byte)GameEventKey.rotation];
			break;
		/// Process to change Players turn
		case (byte)GameEventCode.nextPlayer:
			// Store the received isNextPlayer
			isNextPlayer = (bool)evData[(byte)GameEventKey.boolNext];
			break;
		/// Process to get Ball stuff from opponent 
		case (byte)GameEventCode.StyleNoti:
			// Store the received style
			turnStyle = (int)evData[(byte)GameEventKey.style];
			break;
		}
	}

	/// <summary>
	/// Wraps a call to LiteLobbyPeer.OpJoinFromLobby with an even nicer signature and adds the properties as needed.
	/// This method changes the ChatState to JoiningChatRoom.
	/// </summary>
	public void JoinRoomFromLobby(string roomName)
	{
		this.RoomName = roomName;
		this.AppState = NetworkStateOption.JoiningRoom;
		this.ActorProperties = new Hashtable();

		Hashtable props = new Hashtable() { { NetworkActorProperties.Name, this.UserName } };
		this.Peer.OpJoinFromLobby(this.RoomName, this.LobbyName, props, true);
	}

	/// <summary>
	/// This wrapper method joins a lobby and set the fitting state (this helps us remember what sort of room we joined).
	/// </summary>
	public void JoinLobby(string lobbyName)
	{
		this.LobbyName = lobbyName;
		this.RoomName = lobbyName;
		this.AppState = NetworkStateOption.JoiningLobbyRoom;
		this.RoomHashtable = new Hashtable();
		this.Peer.OpJoin(lobbyName);
	}

	/// <summary>
	/// Calls OpLeave (which can be used in lobby and any other room alike) and sets a state.
	/// </summary>
	public void LeaveRoom()
	{
		this.Peer.OpLeave();
		this.AppState = NetworkStateOption.OutsideOfRooms;
	}

	/// <summary>
	/// In our demo, you can't really disconnect. Only "surplus" clients that we don't want to use anymore do disconnect.
	/// </summary>
	public void Disconnect()
	{
		this.Peer.Disconnect();
	}

	/// Uses OpRaiseEvent to get the message across and adds it to the local chat buffer.
	/// Force to Ball
	public void SendForceToBall(Vector3 Force)
	{
		Hashtable gameEvent = new Hashtable(); // the custom event's data.
		gameEvent.Add((byte)GameEventKey.force, Force);  // add some content
		this.Peer.OpRaiseEvent((byte)GameEventCode.ForceApplied, gameEvent, true);  // call raiseEvent with our content and an event code
	}

	/// Angle for cue to rotate
	public void SendCueAngle(float Angle)
	{
		Hashtable gameEvent = new Hashtable(); // the custom event's data
		gameEvent.Add((byte)GameEventKey.rotation, Angle);
		this.Peer.OpRaiseEvent((byte)GameEventCode.CueRotation, gameEvent, false); // call raiseEvent with our content and anevent code
	}

	/// Position of cue
	public void SendCuePos(Vector3 Pos)
	{
		Hashtable gameEvent = new Hashtable(); // the custom event's data
		gameEvent.Add((byte)GameEventKey.dragging, Pos);
		this.Peer.OpRaiseEvent((byte)GameEventCode.CueDragging, gameEvent, false); // call raiseEvent with our content and an event code
	}

	/// Boolean param for next turn
	public void SendTurnChange(bool nextTurn)
	{
		Hashtable gameEvent = new Hashtable(); // the custom event's data
		gameEvent.Add((byte)GameEventKey.boolNext, nextTurn);
		this.Peer.OpRaiseEvent((byte)GameEventCode.nextPlayer, gameEvent, true); // call raise Event with our content and an event code
	}

	/// Style sends to the opponent
	public void SendStyle(int style)
	{
		Hashtable gameEvent = new Hashtable(); // the custom event's data
		gameEvent.Add((byte)GameEventKey.style, style);
		this.Peer.OpRaiseEvent((byte)GameEventCode.StyleNoti, gameEvent, true); // call raise Event with our content and an event code
	}

	/// <summary>
	/// Helper method to find a name inside the cached actor properties.
	/// </summary>
	public string GetActorPropertyNameOf(int actorNr)
	{
		if (this.ActorProperties == null)
		{
			return "ActorProperties == null";
		}
		
		// we cache all actor properties in this.ActorProperties and each actor has it's own hashtable of properties
		Hashtable actorProps = this.ActorProperties[actorNr] as Hashtable;
		if (actorProps != null)
		{
			// if we found this user's properties, get the name property from it (we defined this in this demo ourselfs)
			string name = actorProps[(byte)NetworkActorProperties.Name] as string;
			if (!String.IsNullOrEmpty(name))
			{
				return name;
			}
		}
		
		// fallback if user is not found
		return String.Format("{0} unknown", actorNr);
	}
}

