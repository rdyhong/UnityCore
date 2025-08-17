using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;

public class PhotonMgr : Singleton<PhotonMgr>
{
    public IEnumerator InitializeCo()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = $"{Application.version}";
        Debug.Log($"{Application.version}");

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
        PhotonPlayerMgr.Inst.SetNickName($"{SteamFriends.GetPersonaName()}");
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
