using MyProtocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.CREATE_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.CREATE_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;
            default:
                break;
        }
    }

    private InputField inpName;
    private Button btnCreate;

    private PromptMsg promptMsg;
    private SocketMsg socketMsg;

    void Start()
    {
        inpName = transform.Find("inpName").GetComponent<InputField>();
        btnCreate = transform.Find("btnCreate").GetComponent<Button>();

        btnCreate.onClick.AddListener(CreateClick);

        promptMsg = new PromptMsg();
        socketMsg = new SocketMsg();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        btnCreate.onClick.RemoveListener(CreateClick);
    }

    private void CreateClick()
    {
        if (string.IsNullOrEmpty(inpName.text))
        {
            // illegal name 
            promptMsg.Change("请正确输入您的昵称", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }

        
        socketMsg.Change(OpCode.USER, UserCode.CREATE_CREQ, inpName.text);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

}
