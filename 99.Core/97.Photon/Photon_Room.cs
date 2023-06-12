using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public partial class Photon_Room : MonoBehaviourPunCallbacks
{
    

    private void Awake()
    {
        PhotonMgr.room = this;
    }

    public static bool isInRoom { get { return PhotonNetwork.InRoom; } }
    public static bool isMasterClient { get { return PhotonNetwork.IsMasterClient; } }

    public static void CreatedRoom(string _roomName = null, RoomOptions _option = null, Action _callback = null)
    {
        PhotonMgr.OnWorkingBlock();

        createRoomCB = _callback;
        PhotonNetwork.CreateRoom(_roomName, _option, null);
    }

    public static void JoinRoom(string _roomName = null)
    {
        PhotonMgr.OnWorkingBlock();

        foreach(RoomInfo _info in GetAllRoomInfo())
        {
            if(_info.Name == _roomName)
            {
                PhotonNetwork.JoinRoom(_roomName);
                break;
            }
        }
        PhotonMgr.OnWorking = false;
        NoticeMgr.AddNotice($"Room is not found\n'{_roomName}'", "Room not found");
    }
    public static void JoinRandomRoom(Action _joinRandomRoomCB = null)
    {
        PhotonMgr.OnWorkingBlock();

        if (isInRoom)
        {
            NoticeMgr.AddNotice("Already In Room", "Fail");
            return;
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public static RoomOptions SetRoomOption(int _maxPlayer = 4, bool _isOpen = true, bool _isVisible = true)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = _maxPlayer;
        options.IsOpen = _isOpen;
        options.IsVisible = _isVisible;

        return options;
    }

    public static void LeaveRoom()
    {
        PhotonMgr.OnWorkingBlock();

        if (!isInRoom)
        {
            DebugMgr.LogErr("Not In Room");
            PhotonMgr.OnWorking = false;
            return;
        }

        PhotonNetwork.LeaveRoom();
    }

    public static string GetRoomName()
    {
        return PhotonNetwork.CurrentRoom.Name;
    }
    public static int GetRoomMaxPlayer()
    {
        return PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    public static int GetRoomCurruntPlayerCount()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }
    private static List<RoomInfo> roomList = new List<RoomInfo>();
    public static List<RoomInfo> GetAllRoomInfo()
    {
        return roomList;
    }

    public static Player[] GetAllPlayer()
    {
        return PhotonNetwork.PlayerList;
    }
    public static bool IsAllReady()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i > players.Length; i++)
        {
            //players[i].SetCustomProperties(new Hashtable() { { "Room_Ready", false } });
            bool isReady = (bool)players[i].CustomProperties["Room_Ready"];
            if (!isReady)
            {
                return false;
            }
        }

        return true;
    }
}
