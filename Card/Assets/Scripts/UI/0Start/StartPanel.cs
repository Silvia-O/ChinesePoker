using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBase
{

    private Button btnStart;
    private Button btnQuit;
    
    // Start is called before the first frame update
    void Start()
    {
        btnStart = transform.Find("btnStart").GetComponent<Button>();
        btnQuit = transform.Find("btnQuit").GetComponent<Button>();
        
        btnStart.onClick.AddListener(StartClick);
        btnQuit.onClick.AddListener(QuitClick);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        btnStart.onClick.RemoveAllListeners();
        btnQuit.onClick.RemoveAllListeners();
    }
    private void StartClick()
    {
        Dispatch(AreaCode.UI, UIEvent.READY_PANEL_ACTIVE, true);
    }

    private void QuitClick()
    {
        Application.Quit();
    }
}
