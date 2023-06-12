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

    

    public static void Init(Action _callback)
    {
        controller.Init(_callback);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Photon_Controller.LoadScene(SceneKind.Lobby);
            SceneMgr.Inst.LoadScene(SceneKind.Lobby);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Photon_Room.LeaveRoom();
        }
    }

    public static void OnWorkingBlock()
    {
        Inst.blockPanel.SetActive(true);
        OnWorking = true;

        Util.waitUntil(() => !OnWorking, () => {
            OnWorking = false;
            Inst.blockPanel.SetActive(false);
        });
    }
}
