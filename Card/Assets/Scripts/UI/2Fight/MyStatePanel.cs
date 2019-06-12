using MyProtocol.Code;
using MyProtocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyStatePanel : StatePanel
{
    //private void Awake()
    protected override void Awake()
    {
        base.Awake();
        Bind(UIEvent.SHOW_GRAB_BUTTON,
            UIEvent.SHOW_DEAL_BUTTON,
            UIEvent.PLAYER_HIDE_READY_BUTTON,
            UIEvent.HIDE_DEAL_BUTTON);
    }

    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);

        switch (eventCode)
        {
            case UIEvent.SHOW_GRAB_BUTTON:
                {
                    bool active = (bool)message;
                    btnGrab.gameObject.SetActive(active);
                    btnNGrab.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.SHOW_DEAL_BUTTON:
                {
                    bool active = (bool)message;
                    btnDeal.gameObject.SetActive(active);
                    btnNDeal.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.PLAYER_HIDE_READY_BUTTON:
                {
                    btnReady.gameObject.SetActive(false);
                    break;
                }
            case UIEvent.HIDE_DEAL_BUTTON:
            {
                HideDealClick();
                break;
            }
            default:
                break;
        }
    }

    private Button btnDeal;
    private Button btnNDeal;
    private Button btnGrab;
    private Button btnNGrab;
    private Button btnReady;
    private Button btnLeave;

    private SocketMsg socketMsg;

    protected override void Start()
    {
        base.Start();

        btnDeal = transform.Find("btnDeal").GetComponent<Button>();
        btnNDeal = transform.Find("btnNDeal").GetComponent<Button>();
        btnGrab = transform.Find("btnGrab").GetComponent<Button>();
        btnNGrab = transform.Find("btnNGrab").GetComponent<Button>();
        btnReady = transform.Find("btnReady").GetComponent<Button>();
        btnLeave = transform.Find("btnLeave").GetComponent<Button>();

        btnDeal.onClick.AddListener(DealClick);
        btnNDeal.onClick.AddListener(NDealClick);
        //btnLeave.onClick.AddListener(LeaveClick);

        btnGrab.onClick.AddListener(
            delegate ()
            {
                GrabClick(true);
            }
            );
        btnNGrab.onClick.AddListener(
            () =>
            {
                GrabClick(false);
            });

        btnReady.onClick.AddListener(ReadyClick);

        socketMsg = new SocketMsg();


        // default
        btnGrab.gameObject.SetActive(false);
        btnNGrab.gameObject.SetActive(false);
        btnDeal.gameObject.SetActive(false);
        btnNDeal.gameObject.SetActive(false);

        UserDto myUserDto = Models.GameModel.MatchRoomDto.UIdUserDict[Models.GameModel.UserDto.Id];
        this.userDto = myUserDto;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        btnDeal.onClick.RemoveAllListeners();
        btnNDeal.onClick.RemoveAllListeners();
        btnGrab.onClick.RemoveAllListeners();
        btnNGrab.onClick.RemoveAllListeners();
        btnReady.onClick.RemoveAllListeners();
    }

    protected override void ReadyState()
    {
        base.ReadyState();
        btnReady.gameObject.SetActive(false);
    }

    private void DealClick()
    {
        Dispatch(AreaCode.CHARACTER, CharacterEvent.DEAL_CARD, null);

    }

    private void HideDealClick()
    {
        btnDeal.gameObject.SetActive(false);
        btnNDeal.gameObject.SetActive(false);
    }

    private void NDealClick()
    {
        socketMsg.Change(OpCode.FIGHT, FightCode.PASS_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        btnDeal.gameObject.SetActive(false);
        btnNDeal.gameObject.SetActive(false);
    }

    private void GrabClick(bool result)
    {
        if (result == true)
        {
            socketMsg.Change(OpCode.FIGHT, FightCode.GRAB_LANDLORD_CREQ, true);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }
        else
        {
            socketMsg.Change(OpCode.FIGHT, FightCode.GRAB_LANDLORD_CREQ, false);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }

        btnGrab.gameObject.SetActive(false);
        btnNGrab.gameObject.SetActive(false);
    }

    private void ReadyClick()
    {
        socketMsg.Change(OpCode.MATCH, MatchCode.READY_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    /*private void LeaveClick()
    {
        socketMsg.Change(OpCode.FIGHT, MatchCode.LEAVE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        socketMsg.Change(OpCode.USER, UserCode.GET_INFO_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(1, null));
    }*/

}
