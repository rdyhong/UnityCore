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

    Action masterConnectCB;
    
    
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        PhotonMgr.controller = this;
    }
    public void Init(Action _callback = null)
    {
        masterConnectCB = _callback;

        PhotonMgr.OnWorkingBlock();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void LoadScene(SceneKind _scene)
    {
        photonView.RPC("RPC_LoadScene", RpcTarget.All);
        
    }
    [PunRPC]
    void RPC_LoadScene()
    {
        PhotonMgr.OnWorkingBlock();

        SceneMgr.Inst.LoadScene(SceneKind.InGame);
    }

    public static int GetPing()
    {
        return PhotonNetwork.GetPing();
    }
    

    #region CallBacks

    public override void OnConnectedToMaster()
    {
        masterConnectCB?.Invoke();
        masterConnectCB = null;

        PhotonMgr.OnWorking = false;

        PhotonNetwork.JoinLobby();

        DebugMgr.Log("Photon ::: OnConnectedToMasterServer");
    }
    public override void OnConnected()
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log("Photon ::: OnConnected");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log($"Photon ::: OnDisconnected by {cause}");
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        DebugMgr.Log($"Photon ::: OnMasterClientSwitched -> {newMasterClient}");
    }

    // Lobby

    public override void OnJoinedLobby()
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log("Photon ::: OnJoinedLobby");
    }
    public override void OnLeftLobby()
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log("Photon ::: OnLeftLobby");
    }
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        DebugMgr.Log("Photon ::: OnLobbyStatisticsUpdate");
    }
    // Room

    public override void OnCreatedRoom()
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log("Photon ::: OnCreatedRoom");
    }
    public override void OnJoinedRoom()
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log("Photon ::: OnJoinedRoom");
    }
    public override void OnLeftRoom()
    {
        PhotonMgr.OnWorking = false;
        DebugMgr.Log("Photon ::: OnLeftRoom");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DebugMgr.Log("Photon ::: OnPlayerEnteredRoom");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DebugMgr.Log("Photon ::: OnPlayerLeftRoom");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        DebugMgr.Log("Photon ::: OnRoomListUpdate");
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        DebugMgr.Log("Photon ::: OnRoomPropertiesUpdate");
    }

    // Room Error
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        NoticeMgr.AddNotice($"{message}", "Create Room Failed");
        PhotonMgr.OnWorking = false;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        NoticeMgr.AddNotice($"{message}", "Join Room Failed");
        PhotonMgr.OnWorking = false;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        NoticeMgr.AddNotice($"{message}", "Join RandomRoom Failed");
        PhotonMgr.OnWorking = false;
    }

    // Error Callback

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        NoticeMgr.AddNotice($"{errorInfo.Info}", "Error");
        PhotonMgr.OnWorking = false;
    }
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        NoticeMgr.AddNotice($"{debugMessage}", "Custom Authentication Failed");
        PhotonMgr.OnWorking = false;
    }

    // Other
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        DebugMgr.Log("Photon ::: OnCustomAuthenticationResponse");
    }
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        DebugMgr.Log("Photon ::: OnFriendListUpdate");
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        DebugMgr.Log("Photon ::: OnPlayerPropertiesUpdate");
    }
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        DebugMgr.Log("Photon ::: OnRegionListReceived");
    }
    public override void OnWebRpcResponse(OperationResponse response)
    {
        DebugMgr.Log("Photon ::: OnWebRpcResponse");
    }
    #endregion
}
