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
    private Text txtMutiple;
    private Button btnChat;
    private Image imgChoose;
    private Button[] btns;

    private SocketMsg socketMsg;

    private void Start()
    {
        initPanel();
        socketMsg = new SocketMsg();

        // default
        imgChoose.gameObject.SetActive(false);

        RefreshBean(Models.GameModel.UserDto.Bean);
    }

    private void initPanel()
    {
        txtBean = transform.Find("txtBean").GetComponent<Text>();
        txtMutiple = transform.Find("txtMutiple").GetComponent<Text>();
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
        txtMutiple.text = "倍数 × " + mutiple;
    }
}
