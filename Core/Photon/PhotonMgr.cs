using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMgr : Singleton<PhotonMgr>
{
    [SerializeField] GameObject blockPanel;
    public static bool OnWorking = false;

    public static Photon_Controller controller;
    public static Photon_Room room;

    public IEnumerator InitializeCo()
    {
        controller = new GameObject(nameof(Photon_Controller)).AddComponent<Photon_Controller>();
        controller.transform.SetParent(transform);
       // controller.Initialize();

        PhotonNetwork.ConnectUsingSettings();

        // 클라이언트 세팅 완료
        Debug.Log("ConnectUsingSettings");
        yield return PhotonNetwork.IsConnectedAndReady;

        // 마스터 서버 접속 대기
        Debug.Log("Try Connect To Master Server...");
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
        
        // 로비 접속 대기
        PhotonNetwork.JoinLobby();
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);
        Debug.Log("JoinedLobby");
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Photon_Controller.LoadScene(SceneKind.Lobby);
            //SceneMgr.Inst.LoadScene(EScene.LobbyScene);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Photon_Room.LeaveRoom();
        }
    }

    public static int GetPing()
    {
        return PhotonNetwork.GetPing();
    }
    public static Player[] GetAllPlayer()
    {
        return PhotonNetwork.PlayerList;
    }
    public IEnumerator JoinRandomRoomCo()
    {
        if (!PhotonNetwork.IsConnectedAndReady ||
            PhotonNetwork.NetworkClientState == ClientState.Joining ||
            PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            Debug.LogWarning("다른 작업 중입니다.");
            yield break;
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public void CreatedRoom(string _roomName = null, RoomOptions _option = null)
    {
        // 포톤이 연결되어 있는지 & 상태가 방 생성을 할 수 있는지 체크
        if (!PhotonNetwork.IsConnectedAndReady ||
            PhotonNetwork.NetworkClientState == ClientState.Joining ||
            PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            Debug.LogWarning("다른 작업 중입니다.");
            return;
        }

        PhotonNetwork.CreateRoom(_roomName, _option, null);
    }
}
