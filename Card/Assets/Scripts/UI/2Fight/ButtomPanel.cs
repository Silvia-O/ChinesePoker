using MyProtocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtomPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.CHANGE_MUTIPLE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.CHANGE_MUTIPLE:
                ChangeMutiple((int)message);
                break;
            default:
                break;
        }
    }

    private Text txtBean;
    private Text txtMultiple;

    private SocketMsg socketMsg;

    private void Start()
    {
        InitPanel();
        socketMsg = new SocketMsg();


        RefreshBean(Models.GameModel.UserDto.Bean);
    }

    private void InitPanel()
    {
        txtBean = transform.Find("txtBean").GetComponent<Text>();
        txtMultiple = transform.Find("txtMultiple").GetComponent<Text>();
    }

    /// <summary>
    /// refresh self state panel
    /// </summary>
    private void RefreshBean(int beenCount)
    {
        this.txtBean.text = "× " + beenCount;
    }

    /// <summary>
    /// change game multiple
    /// </summary>
    /// <param name="mutiple"></param>
    private void ChangeMutiple(int mutiple)
    {
        txtMultiple.text = "倍数 × " + mutiple;
    }
}
