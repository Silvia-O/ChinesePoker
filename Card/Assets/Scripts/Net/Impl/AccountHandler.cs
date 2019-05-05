using MyProtocol.Code;
using UnityEngine;

public class AccountHandler: HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case AccountCode.LOGIN_SRES:
                LoginResponse((int) value);
                break;

            case AccountCode.REGISTER_SRES:
                RegisterResponse((int) value);
                break;
            default:
                break;
        }
    }
    
    private PromptMsg promptMsg = new PromptMsg();

    private void LoginResponse(int result)
    {
        switch (result)
        {
            case 0:
                // convert scene
                LoadSceneMsg msg = new LoadSceneMsg(1,
                    delegate ()
                    {
                        // get msg from server
                        SocketMsg socketMsg = new SocketMsg(OpCode.USER, UserCode.GET_INFO_CREQ, null);
                        Dispatch(AreaCode.NET, 0, socketMsg);
                        Debug.Log("成功登录!");
                    });
                Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);
                break;
            case -1:
                promptMsg.Change("用户不存在!", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            case -2:
                promptMsg.Change("用户已在线!", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            case -3:
                promptMsg.Change("用户名密码不匹配!", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            default:
                break;
        }
    }

    public void RegisterResponse(int result)
    {
        switch (result)
        {
            case 0:
                promptMsg.Change("成功注册!", Color.green);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            case -1:
                promptMsg.Change("用户已存在!", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            case -2:
                promptMsg.Change("用户名不合法!", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            case -3:
                promptMsg.Change("密码不合法!", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
                break;
            default:
                break;
        }
    }
}