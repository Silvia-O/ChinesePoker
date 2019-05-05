using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyProtocol.Code;
using MyProtocol.Dto;

public class UserHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case UserCode.CREATE_SRES:
                CreateResponse((int)value);
                break;
            case UserCode.GET_INFO_SRES:
                GetInfoResponse(value as UserDto);
                break;
            case UserCode.ONLINE_SRES:
                OnlineResponse((int)value);
                break;
            default:
                break;
        }
    }

    private SocketMsg socketMsg = new SocketMsg();

    private void GetInfoResponse(UserDto user)
    {
        if (user == null)
        {
            // no character
            Debug.Log("No character. To create.");
            // activate create panel
            Dispatch(AreaCode.UI, UIEvent.CREATE_PANEL_ACTIVE, true);
        }
        else
        {
            // deactivate create panel
            Dispatch(AreaCode.UI, UIEvent.CREATE_PANEL_ACTIVE, false);

            // online character
            //socketMsg.Change(OpCode.USER, UserCode.ONLINE_CREQ, null);
            //Dispatch(AreaCode.NET, 0, socketMsg);

            
            //GameModel model = new GameModel();
            Models.GameModel.UserDto = user;

            // refresh info panel
            Dispatch(AreaCode.UI, UIEvent.REFRESH_INFO_PANEL, user);
        }
    }

    private void OnlineResponse(int result)
    {
        if(result == 0)
        {
            Debug.Log("Success to online!");
        }
        else if(result == -1)
        {
            Debug.LogError("User illegally login!");
        }
        else if(result == -2)
        {
            Debug.LogError("User illegally online!");
        }
    }

    private void CreateResponse(int result)
    {
        if(result == -1)
        {
            Debug.LogError("User illegally login!");
        }
        else if(result == -2)
        {
            Debug.LogError("Character already exists!");
        }
        else if(result == 0)
        {
            // deactivate create panel
            Dispatch(AreaCode.UI, UIEvent.CREATE_PANEL_ACTIVE, false);
            // get user info
            socketMsg.Change(OpCode.USER, UserCode.GET_INFO_CREQ, null);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }
    }

}
