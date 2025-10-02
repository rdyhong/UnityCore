using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;

public enum ELobbyType
{
    QuickMatch = 0,
    CustomMatch,
    Rank
}


public class PhotonMgr : Singleton<PhotonMgr>
{
    private IEnumerator _joinLobbyCo = null;

    public IEnumerator InitializeCo()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = $"{Application.version}";
        Debug.Log($"{Application.version}");

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();

        // 클라이언트 세팅 완료
        Debug.Log("ConnectUsingSettings");
        yield return PhotonNetwork.IsConnectedAndReady;

        // 마스터 서버 접속 대기
        Debug.Log("Try Connect To Master Server...");
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        PhotonRoomMgr.Inst.Initialize();

        PhotonPlayerMgr.Inst.SetNickName($"{SteamFriends.GetPersonaName()}");

        // 로비 접속 대기
        //PhotonNetwork.JoinLobby();
        //yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);
        //Debug.Log("JoinedLobby");
        
        Debug.Log("포톤 초기화 성공");
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

    //public void JoinLobby(ELobbyType lobbyType)
    //{
    //    if (_joinLobbyCo != null) return;

    //    _joinLobbyCo = JoinLobbyCo(lobbyType);
    //    StartCoroutine(_joinLobbyCo);
    //}

    public IEnumerator JoinLobbyCo(ELobbyType lobbyType)
    {
        // 현재 로비 타입 확인
        if (PhotonNetwork.InLobby)
        {
            // 같은 로비면 스킵
            if (PhotonNetwork.CurrentLobby?.Name == lobbyType.ToString())
            {
                Debug.Log($"Already in {lobbyType} lobby");
                _joinLobbyCo = null;
                yield break;
            }

            PhotonNetwork.LeaveLobby();
            yield return new WaitUntil(() => !PhotonNetwork.InLobby);
            Debug.Log($"Leave Lobby");
            yield return null;
        }

        TypedLobby lobbyTyped = new TypedLobby(lobbyType.ToString(), LobbyType.Default);
        PhotonNetwork.JoinLobby(lobbyTyped);
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);
        Debug.Log($"Joined Lobby({lobbyType})");
        _joinLobbyCo = null;
    }

    public static int GetPing()
    {
        return PhotonNetwork.GetPing();
    }
    
    public static Player[] GetAllPlayer()
    {
        return PhotonNetwork.PlayerList;
    }

    public static T GetObjectFromViewId<T>(int viewId) where T : class
    {
        PhotonView foundPv = PhotonView.Find(viewId);
        if (foundPv == null) return null;

        return PhotonView.Find(viewId).GetComponent<T>();
    }
}
