using MyProtocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏数据的存储类
/// </summary>
public class GameModel
{
    public UserDto UserDto { get; set; }

    public int Id { get { return UserDto.Id; } }

    public MatchRoomDto MatchRoomDto { get; set; }

    public UserDto GetUserDto(int userId)
    {
        return MatchRoomDto.UIdUserDict[userId];
    }
    
    public int GetRightUserId()
    {
        return MatchRoomDto.RightId;
    }

}
