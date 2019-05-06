using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MyProtocol.Code;
using MyProtocol.Dto;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanel : UIBase
{

    private void Awake()
    {
        Bind(UIEvent.READY_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object msg)
    {
        switch (eventCode)
        {
            case(UIEvent.READY_PANEL_ACTIVE):
                setPanelActive((bool) msg);
                break;
            default:
                break;
        }
    }
    private Button btnLogin;
    private Button btnRegister;
    private Button btnClose;
    private InputField inpAcc;
    private InputField inpPwd;

    private PromptMsg promotMsg;
    private SocketMsg socketMsg;
    // Start is called before the first frame update
    void Start()
    {
        btnLogin = transform.Find("btnLogin").GetComponent<Button>();
        btnRegister = transform.Find("btnRegister").GetComponent<Button>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        inpAcc = transform.Find("inpAcc").GetComponent<InputField>();
        inpPwd = transform.Find("inpPw").GetComponent<InputField>();
        
        btnLogin.onClick.AddListener(LoginClick);
        btnRegister.onClick.AddListener(RegisterClick);
        btnClose.onClick.AddListener(CloseClick);
        
        promotMsg = new PromptMsg();
        socketMsg = new SocketMsg();
        
        // default hidden
        setPanelActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        btnLogin.onClick.RemoveListener(LoginClick);
        btnRegister.onClick.RemoveListener(RegisterClick);
        btnClose.onClick.RemoveListener(CloseClick);
    }
    
    private void LoginClick()
    {
        if (string.IsNullOrEmpty(inpAcc.text))
        {
            promotMsg.Change("用户名不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promotMsg);
            return;
        }

        if (string.IsNullOrEmpty(inpPwd.text))
        {
            promotMsg.Change("密码不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promotMsg);
            return;
        }
        if (inpPwd.text.Length < 6
            || inpPwd.text.Length > 16)
        {
            promotMsg.Change("密码长度需为6~16位!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promotMsg);
            return;
        } 
        
        // interact with server
        AccountDto dto = new AccountDto(inpAcc.text, inpPwd.text);
        SocketMsg msg = new SocketMsg(OpCode.ACCOUNT, AccountCode.LOGIN_CREQ, dto);
        Dispatch(AreaCode.NET, 0, msg);
    }

    private void RegisterClick()
    {

        if (string.IsNullOrEmpty(inpAcc.text))
        {
            promotMsg.Change("用户名不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promotMsg);
            return;
        }
        if (string.IsNullOrEmpty(inpPwd.text))
        {
            promotMsg.Change("密码不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promotMsg);
            return;
        }
        if (inpPwd.text.Length < 6
            || inpPwd.text.Length > 16)
        {
            promotMsg.Change("密码长度需为6~16位!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promotMsg);
            return;
        } 
        
        // interact with server
        AccountDto dto = new AccountDto(inpAcc.text, inpPwd.text);
        socketMsg.Change(OpCode.ACCOUNT, AccountCode.REGISTER_CREQ, dto);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    private void CloseClick()
    {
        setPanelActive(false);
    }
    
}
