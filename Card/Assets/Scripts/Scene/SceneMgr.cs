using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneMgr : ManagerBase
{
    public static SceneMgr Instance = null;

    private void Awake()
    {
        Instance = this;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        Add(SceneEvent.LOAD_SCENE, this);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case SceneEvent.LOAD_SCENE:
                LoadSceneMsg msg = message as LoadSceneMsg;
                LoadScene(msg);
                break;
            default:
                break;
        }
    }
    
    private Action tmpOnSceneLoaded = null;
    private void LoadScene(LoadSceneMsg msg)
    {
        if (msg.SceneBuildIndex != -1)
            SceneManager.LoadScene(msg.SceneBuildIndex);

        if (msg.SceneName != null)
            SceneManager.LoadScene(msg.SceneName);

        if (msg.OnSceneLoaded != null)
            tmpOnSceneLoaded = msg.OnSceneLoaded;
    }

    /// <summary>
    /// call when scene has been loaded
    /// </summary>
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (tmpOnSceneLoaded != null)
        {
            tmpOnSceneLoaded();
            tmpOnSceneLoaded = null;
        }
    }
}