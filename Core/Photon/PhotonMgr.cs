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

    public void Initialize()
    {
        //controller = new GameObject(nameof(Photon_Controller)).AddComponent<Photon_Controller>();
        //controller.Initialize();

        //PhotonNetwork.ConnectUsingSettings();
    }

    public IEnumerator InitializeCo()
    {
        controller = new GameObject(nameof(Photon_Controller)).AddComponent<Photon_Controller>();
        controller.transform.SetParent(transform);
       // controller.Initialize();

        PhotonNetwork.ConnectUsingSettings();

        // 클라이언트 세팅 완료
        yield return PhotonNetwork.IsConnectedAndReady;
        Debug.Log("ConnectedAndReady");

        // 마스터 서버 접속 대기
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
        Debug.Log("ConnectedToMasterServer");


    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Photon_Controller.LoadScene(SceneKind.Lobby);
            SceneMgr.Inst.LoadScene(EScene.Lobby);
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

    public static void OnWorkingBlock()
    {
        Inst.blockPanel.SetActive(true);
        OnWorking = true;

        GameUtil.WaitUntil(() => !OnWorking, () => {
            OnWorking = false;
            Inst.blockPanel.SetActive(false);
        });
    }
}
