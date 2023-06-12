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
    private SceneKind loadSceneKind = SceneKind.None;
    private Dictionary<SceneKind, GameScene> gameScenes = new Dictionary<SceneKind, GameScene>();
    private LoadCallback loadCallback;
    
    public GameScene curScene;
    public static SceneKind curSceneKind;

    protected override void Awake()
    {
        base.Awake();

        gameScenes.Add(SceneKind.Splash, new Splash());
        gameScenes.Add(SceneKind.Lobby, new Lobby());
        gameScenes.Add(SceneKind.Title, new Title());
        gameScenes.Add(SceneKind.InGame, new InGame());
    }

    public void LoadScene(SceneKind sceneKind, LoadCallback callback = null)
    {
        loadSceneKind = sceneKind;
        loadCallback = callback;
        curScene = gameScenes[sceneKind];
        if (sceneKind == SceneKind.None)
            return;

        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneKind.ToString());

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        curScene.DataInitialize(() =>
        {
            GC.Collect();
        });

        curScene.Init();

        loadCallback?.Invoke();

        curSceneKind = loadSceneKind;

        PhotonMgr.OnWorking = false;
    }
}