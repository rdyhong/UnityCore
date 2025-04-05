using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonRoomMgr : SingletonPun<PhotonRoomMgr>
{
    private Action<bool> _resultCb = null;
    
    public void CreateRoom(string roomName = null, RoomOptions option = null, Action<bool> resultCb = null)
    {
        // 포톤이 연결되어 있는지 & 상태가 방 생성을 할 수 있는지 체크
        if (!PhotonNetwork.IsConnectedAndReady ||
            PhotonNetwork.NetworkClientState == ClientState.Joining ||
            PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            Debug.LogWarning("다른 작업 중입니다.");
            return;
        }
        _resultCb = resultCb;
        PhotonNetwork.CreateRoom(roomName, option, null);
    }

    public RoomOptions CreateRoomOption(int _maxPlayer = 4, bool _isOpen = true, bool _isVisible = true)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = _maxPlayer;
        options.IsOpen = _isOpen;
        options.IsVisible = _isVisible;
        return options;
    }

    public override void OnCreatedRoom()
    {
        _resultCb?.Invoke(true);
        _resultCb = null;
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _resultCb?.Invoke(false);
        _resultCb = null;
        Debug.Log($"{returnCode}:{message}");
    }

    public void JoinRandomRoom(Action<bool> resultCb = null)
    {
        _resultCb = resultCb;
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom(string roomName, Action<bool> resultCb = null)
    {
        _resultCb = resultCb;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        _resultCb?.Invoke(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        _resultCb?.Invoke(false);
        Debug.Log($"{returnCode}:{message}");
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _resultCb?.Invoke(false);
        Debug.Log($"{returnCode}:{message}");
    }
}
