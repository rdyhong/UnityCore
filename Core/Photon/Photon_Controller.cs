using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public partial class Photon_Controller : MonoBehaviourPunCallbacks
{
    PhotonView pv;

    #region CallBacks

    public override void OnConnectedToMaster()
    {

        //PhotonNetwork.JoinLobby();

        Debug.Log("Photon ::: OnConnectedToMasterServer");
    }
    public override void OnConnected()
    {
        DebugUtil.Log("Photon ::: OnConnected");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        DebugUtil.Log($"Photon ::: OnDisconnected by {cause}");
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        DebugUtil.Log($"Photon ::: OnMasterClientSwitched -> {newMasterClient}");
    }

    // Lobby

    public override void OnJoinedLobby()
    {
        DebugUtil.Log("Photon ::: OnJoinedLobby");
    }
    public override void OnLeftLobby()
    {
        DebugUtil.Log("Photon ::: OnLeftLobby");
    }
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        DebugUtil.Log("Photon ::: OnLobbyStatisticsUpdate");
    }
    // Room

    public override void OnCreatedRoom()
    {
        DebugUtil.Log("Photon ::: OnCreatedRoom");
    }
    public override void OnJoinedRoom()
    {
        DebugUtil.Log("Photon ::: OnJoinedRoom");
    }
    public override void OnLeftRoom()
    {
        DebugUtil.Log("Photon ::: OnLeftRoom");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DebugUtil.Log("Photon ::: OnPlayerEnteredRoom");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DebugUtil.Log("Photon ::: OnPlayerLeftRoom");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        DebugUtil.Log("Photon ::: OnRoomListUpdate");
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        DebugUtil.Log("Photon ::: OnRoomPropertiesUpdate");
    }

    // Room Error
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        DebugUtil.Log($"{message}, Create Room Failed");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugUtil.Log($"{message}, Join Room Failed");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        DebugUtil.Log($"{message}, Join RandomRoom Failed");
    }

    // Error Callback

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        DebugUtil.Log($"{errorInfo.Info}, Error");
    }
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        DebugUtil.Log($"{debugMessage}, Custom Authentication Failed");
    }

    // Other
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        DebugUtil.Log("Photon ::: OnCustomAuthenticationResponse");
    }
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        DebugUtil.Log("Photon ::: OnFriendListUpdate");
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        DebugUtil.Log("Photon ::: OnPlayerPropertiesUpdate");
    }
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        DebugUtil.Log("Photon ::: OnRegionListReceived");
    }
    public override void OnWebRpcResponse(OperationResponse response)
    {
        DebugUtil.Log("Photon ::: OnWebRpcResponse");
    }
    #endregion
}
