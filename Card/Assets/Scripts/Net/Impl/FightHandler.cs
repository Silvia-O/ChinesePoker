using MyProtocol.Code;
using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case FightCode.GET_CARD_SRES:
                GetCards(value as List<CardDto>);
                break;
            case FightCode.TURN_GRAB_BRO:
                TurnGrabBro((int)value);
                break;
            case FightCode.GRAB_LANDLORD_BRO:
                GrabLandlordBro(value as GrabDto);
                break;
            case FightCode.TURN_DEAL_BRO:
                TurnDealBro((int)value);
                break;
            case FightCode.DEAL_BRO:
                DealBro(value as DealDto);
                break;
            case FightCode.DEAL_SRES:
                DealResponse((int)value);
                break;
            case FightCode.OVER_BRO:
                OverBro(value as OverDto);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// game over broadcast
    /// </summary>
    /// <param name="dto"></param>
    private void OverBro(OverDto dto)
    {
        Dispatch(AreaCode.UI, UIEvent.SHOW_OVER_PANEL, dto);
    }

    /// <summary>
    /// deal response
    /// </summary>
    /// <param name="result"></param>
    private void DealResponse(int result)
    {
        if (result == -1)
        {
            PromptMsg promptMsg = new PromptMsg("管不了上一个玩家出的牌", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
 
            Dispatch(AreaCode.UI, UIEvent.SHOW_DEAL_BUTTON, true);
        }
    }

    /// <summary>
    /// deal broadcast
    /// </summary>
    /// <param name="dto"></param>
    private void DealBro(DealDto dto)
    {
        int eventCode = -1;
        if (dto.UserId == Models.GameModel.MatchRoomDto.LeftId)
        {
            eventCode = CharacterEvent.REMOVE_LEFT_CARD;
        }
        else if (dto.UserId == Models.GameModel.MatchRoomDto.RightId)
        {
            eventCode = CharacterEvent.REMOVE_RIGHT_CARD;
        }
        else if (dto.UserId == Models.GameModel.UserDto.Id)
        {
            eventCode = CharacterEvent.REMOVE_MY_CARD;
        }
        Dispatch(AreaCode.CHARACTER, eventCode, dto.RemainCardList);

        Dispatch(AreaCode.CHARACTER, CharacterEvent.UPDATE_SHOW_DESK, dto.SelectCardList);
      
    }



    /// <summary>
    /// self turn to deal
    /// </summary>
    /// <param name="userId"></param>
    private void TurnDealBro(int userId)
    {
        if (Models.GameModel.Id == userId)
        {
            Dispatch(AreaCode.UI, UIEvent.SHOW_DEAL_BUTTON, true);
        }
    }

    /// <summary>
    /// grab landlord broadcast
    /// </summary>
    private void GrabLandlordBro(GrabDto dto)
    {
        // change identity
        Dispatch(AreaCode.UI, UIEvent.PLAYER_CHANGE_IDENTITY, dto.UserId);
        // show three hidden card
        Dispatch(AreaCode.UI, UIEvent.SET_TABLE_CARDS, dto.TableCardList);
        // show cards of every player
        int eventCode = -1;
        if (dto.UserId == Models.GameModel.MatchRoomDto.LeftId)
        {
            eventCode = CharacterEvent.ADD_LEFT_CARD;
        }
        else if (dto.UserId == Models.GameModel.MatchRoomDto.RightId)
        {
            eventCode = CharacterEvent.ADD_RIGHT_CARD;
        }
        else if (dto.UserId == Models.GameModel.UserDto.Id)
        {
            eventCode = CharacterEvent.ADD_MY_CARD;
        }
        Dispatch(AreaCode.CHARACTER, eventCode, dto);
    }

    /// <summary>
    /// whether is the first one to grab
    /// </summary>
    private bool isFirst = true;

    /// <summary>
    /// self turn to grab
    /// </summary>
    /// <param name="userId"></param>
    private void TurnGrabBro(int userId)
    {
        if (isFirst == true)
        {
            isFirst = false;
        }

        // whether is self turn
        if (userId == Models.GameModel.UserDto.Id)
        {
            Dispatch(AreaCode.UI, UIEvent.SHOW_GRAB_BUTTON, true);
        }
    }

    /// <summary>
    /// get cards
    /// </summary>
    /// <param name="cardList"></param>
    private void GetCards(List<CardDto> cardList)
    {
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_MY_CARD, cardList);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_LEFT_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_RIGHT_CARD, null);

        // set game multiple
        Dispatch(AreaCode.UI, UIEvent.CHANGE_MUTIPLE, 1);
    }
}
