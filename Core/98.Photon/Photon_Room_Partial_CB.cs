using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public partial class Photon_Room : MonoBehaviourPunCallbacks
{
    public static Action createRoomCB = null;
    public static Action joinRoomCB = null;

    public override void OnCreatedRoom()
    {
        createRoomCB?.Invoke();
    }
    public override void OnJoinedRoom()
    {
        joinRoomCB?.Invoke();
    }
    public override void OnLeftRoom()
    {

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public override void OnRoomListUpdate(List<RoomInfo> _roomList)
    {
        roomList.Clear();
        roomList = _roomList;
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
    }
}
