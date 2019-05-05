using MyProtocol.Code;
using MyProtocol.Dto;
using System;
using System.Collections.Generic;

public class MatchHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case MatchCode.ENTER_SRES:
                EnterResponse(value as MatchRoomDto);
                break;
            case MatchCode.ENTER_BRO:
                EnterBroadcast(value as UserDto);
                break;
            case MatchCode.LEAVE_BRO:
                LeaveBroadcast((int)value);
                break;
            case MatchCode.READY_BRO:
                ReadyBroadcast((int)value);
                break;
            case MatchCode.START_BRO:
                StartBroadcast();
                break;
            default:
                break;
        }
    }

    PromptMsg promptMsg = new PromptMsg();

    private void StartBroadcast()
    {
        promptMsg.Change("所有玩家准备开始游戏", UnityEngine.Color.blue);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        //开始游戏 隐藏状态面板的准备文字
        Dispatch(AreaCode.UI, UIEvent.PLAYER_HIDE_STATE, null);
    }

    private void ReadyBroadcast(int readyUserId)
    {
        //保存数据
        Models.GameModel.MatchRoomDto.Ready(readyUserId);
        //显示为玩家状态面板的准备文字
        Dispatch(AreaCode.UI, UIEvent.PLAYER_READY, readyUserId);

        //fixbug923 判断是否是自身
        if(readyUserId  == Models.GameModel.UserDto.Id)
        {
            //发送消息 隐藏准备按钮 防止多次点击 和服务器交互
            Dispatch(AreaCode.UI, UIEvent.PLAYER_HIDE_READY_BUTTON, null);
        }
    }

    
    private void LeaveBroadcast(int leaveUserId)
    {
        Dispatch(AreaCode.UI, UIEvent.PLAYER_LEAVE, leaveUserId);

        ResetPosition();

        Models.GameModel.MatchRoomDto.Leave(leaveUserId);
    }


    private void EnterResponse(MatchRoomDto matchRoom)
    {
        Models.GameModel.MatchRoomDto = matchRoom;
        ResetPosition();
        
        // show enter room button
        Dispatch(AreaCode.UI, UIEvent.SHOW_ENTER_ROOM_BUTTON, null);
    }

  
    private void EnterBroadcast(UserDto newUser)
    {

        // update room info
        MatchRoomDto room = Models.GameModel.MatchRoomDto;
        room.Add(newUser);
        ResetPosition();

        if (room.LeftId != -1)
        {
            UserDto leftUserDto = room.UIdUserDict[room.LeftId];
            Dispatch(AreaCode.UI, UIEvent.SET_LEFT_PLAYER_DATA, leftUserDto);
        }
        if (room.RightId != -1)
        {
            UserDto rightUserDto = room.UIdUserDict[room.RightId];
            Dispatch(AreaCode.UI, UIEvent.SET_RIGHT_PLAYER_DATA, rightUserDto);
        }

        Dispatch(AreaCode.UI, UIEvent.PLAYER_ENTER, newUser.Id);

        promptMsg.Change("有新玩家 ( " + newUser.Name + " )进入", UnityEngine.Color.blue);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
    }

    
    private void ResetPosition()
    {
        GameModel gameModel = Models.GameModel;
        MatchRoomDto matchRoom = gameModel.MatchRoomDto;

        matchRoom.ResetPosition(gameModel.UserDto.Id);
    }

}
