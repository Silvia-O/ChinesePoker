using MyProtocol.Code;
using MyProtocol.Constant;
using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// y游戏结束面板
/// </summary>
public class OverPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.SHOW_OVER_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_OVER_PANEL:
                RefreshPanel(message as OverDto);
                break;
            default:
                break;
        }
    }

    private Text txtWinIdentity;
    private Text txtWinBean;
    private Button btnBack;

    // Use this for initialization
    void Start()
    {
        txtWinIdentity = transform.Find("txtWinIdentity").GetComponent<Text>();
        txtWinBean = transform.Find("txtWinBean").GetComponent<Text>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(BackClick);

        setPanelActive(false);
    }

    /// <summary>
    /// refresh over panel
    /// </summary>
    /// <param name="dto"></param>
    private void RefreshPanel(OverDto dto)
    {
        setPanelActive(true);

        // who wins
        txtWinIdentity.text = Identity.GetString(dto.WinIdentity);
        // whether winner is self
        if (dto.WinUIdList.Contains(Models.GameModel.Id))
        {
            txtWinIdentity.text += "胜利";
            txtWinBean.text = "欢乐豆：+";
        }
        else
        {
            // self lose
            txtWinIdentity.text += "失败";
            txtWinBean.text = "欢乐豆：-";
        }
   
        txtWinBean.text += dto.BeanCount;
    }

    private void BackClick()
    {
        LoadSceneMsg msg = new LoadSceneMsg(1,
                 delegate ()
                 {
                     SocketMsg socketMsg = new SocketMsg(OpCode.USER, UserCode.GET_INFO_CREQ, null);
                     Dispatch(AreaCode.NET, 0, socketMsg);
                 });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);
    }

}
