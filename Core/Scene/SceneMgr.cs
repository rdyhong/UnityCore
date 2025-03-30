using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void LoadCallback();

/// <summary>
/// Manage Load scene
/// </summary>
public class SceneMgr : Singleton<SceneMgr>
{
    private EScene loadSceneKind = EScene.None;
    //private Dictionary<EScene, GameScene> gameScenes = new Dictionary<EScene, GameScene>();
    private LoadCallback loadCallback;
    
    //public GameScene curScene;
    public static EScene curSceneKind;

    protected override void Awake()
    {
        base.Awake();

        //gameScenes.Add(SceneKind.Splash, new Splash());
        //gameScenes.Add(SceneKind.Title, new Title());
        //gameScenes.Add(SceneKind.Lobby, new Lobby());
        //gameScenes.Add(SceneKind.InGame, new InGame());
    }

    public void LoadScene(EScene sceneKind, LoadCallback callback = null)
    {
        if (sceneKind == EScene.None)
            return;

        loadSceneKind = sceneKind;
        loadCallback = callback;
        //curScene = gameScenes[sceneKind];

        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneKind.ToString());

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        //curScene.DataInitialize(() =>
        //{
        //    GC.Collect();
        //});
        GC.Collect();
        //curScene.Init();

        loadCallback?.Invoke();

        curSceneKind = loadSceneKind;
    }
}