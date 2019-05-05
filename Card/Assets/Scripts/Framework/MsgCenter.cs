using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// message manage center
/// forward message
/// </summary>
public class MsgCenter : MonoBase
{
    public static MsgCenter Instance = null;

    void Awake()
    {
        Instance = this;

        gameObject.AddComponent<UIMgr>();
        gameObject.AddComponent<CharacterMgr>();
        gameObject.AddComponent<NetMgr>();
        gameObject.AddComponent<SceneMgr>();
        
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// dispatch message 
    /// different areaCode represents different module
    /// </summary>
    /// <param name="areaCode"></param>
    /// <param name="eventCode"></param>
    /// <param name="message"></param>
    public void Dispatch(int areaCode, int eventCode, object message)
    {
        switch (areaCode)
        {
            case AreaCode.UI:
                UIMgr.Instance.Execute(eventCode, message);
                break;
            
            case AreaCode.GAME:
                break;

            case AreaCode.CHARACTER:
                CharacterMgr.Instance.Execute(eventCode, message);
                break;
            
            case AreaCode.NET:
                NetMgr.Instance.Execute(eventCode, message);
                break;
            
            case AreaCode.SCENE:
                SceneMgr.Instance.Execute(eventCode, message);
                break;
            
            default:
                break;
        }
    }

}