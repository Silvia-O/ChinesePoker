using MyProtocol.Code;
using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerCtrl : CharacterBase
{
    private void Awake()
    {
        Bind(CharacterEvent.INIT_MY_CARD,
            CharacterEvent.ADD_MY_CARD,
            CharacterEvent.DEAL_CARD,
            CharacterEvent.REMOVE_MY_CARD);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.INIT_MY_CARD:
                StartCoroutine(InitCardList(message as List<CardDto>));
                break;
            case CharacterEvent.ADD_MY_CARD:
                AddTableCard(message as GrabDto);
                break;
            case CharacterEvent.DEAL_CARD:
                DealSelectCard();
                break;
            case CharacterEvent.REMOVE_MY_CARD:
                RemoveCard(message as List<CardDto>);
                break;
            default:
                break;
        }
    }

    
    private List<CardCtrl> cardCtrlList;

    private Transform cardParent;

    private PromptMsg promptMsg;
    private SocketMsg socketMsg;

    // Use this for initialization
    void Start()
    {
        cardParent = transform.Find("CardPoint");
        cardCtrlList = new List<CardCtrl>();

        promptMsg = new PromptMsg();
        socketMsg = new SocketMsg();
    }

    /// <summary>
    /// deal selected cards
    /// </summary>
    private void DealSelectCard()
    {
        List<CardDto> selectCardList = GetSelectCard();
        DealDto dto = new DealDto(selectCardList, Models.GameModel.Id);
        // whether it is legal
        if (dto.IsRegular == false)
        {
            promptMsg.Change("请选择合理的手牌！", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            return;
        }
        else
        {
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_CREQ, dto);
            Dispatch(AreaCode.NET, 0, socketMsg);
        }
    }

    /// <summary>
    /// get selected cards
    /// </summary>
    private List<CardDto> GetSelectCard()
    {
        List<CardDto> selectCardList = new List<CardDto>();
        foreach (var cardCtrl in cardCtrlList)
        {
            if (cardCtrl.Selected == true)
            {
                selectCardList.Add(cardCtrl.CardDto);
            }
        }
        return selectCardList;
    }

    /// <summary>
    /// remove card game object
    /// </summary>
    /// <param name="remainCardList"></param>
    private void RemoveCard(List<CardDto> remainCardList)
    {
        int index = 0;
        foreach (var cc in cardCtrlList)
        {
            if (remainCardList.Count == 0)
                break;
            else
            {
                cc.gameObject.SetActive(true);
                cc.InitCards(remainCardList[index], index, true);
                index++;
                // no remain card
                if (index == remainCardList.Count)
                {
                    break;
                }
            }
        }
        // hide card
        for (int i = index; i < cardCtrlList.Count; i++)
        {
            cardCtrlList[i].Selected = false;
            cardCtrlList[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// after adding table card
    /// </summary>
    /// <param name="dto"></param>
    private void AddTableCard(GrabDto dto)
    {
        List<CardDto> tableCards = dto.TableCardList;
        List<CardDto> playerCards = dto.PlayerCardList;

        int index = 0;
        foreach (var cardCtrl in cardCtrlList)
        {
            cardCtrl.gameObject.SetActive(true);
            cardCtrl.InitCards(playerCards[index], index, true);
            index++;
        }
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");
        for (int i = index; i < playerCards.Count; i++)
        {
            CreateGo(playerCards[i], i, cardPrefab);
        }
    }


    /// <summary>
    /// init card list
    /// </summary>
    /// <param name="cardList"></param>
    /// <returns></returns>
    private IEnumerator InitCardList(List<CardDto> cardList)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");

        for (int i = 0; i < cardList.Count; i++)
        {
            CreateGo(cardList[i], i, cardPrefab);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// create card game object
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    /// <param name="cardPrefab"></param>
    private void CreateGo(CardDto card, int index, GameObject cardPrefab)
    {
        GameObject cardGo = Object.Instantiate(cardPrefab, cardParent) as GameObject;
        cardGo.name = card.Name;
        cardGo.transform.localPosition = new Vector2((0.25f * index), 0);
        CardCtrl cardCtrl = cardGo.GetComponent<CardCtrl>();
        cardCtrl.InitCards(card, index, true);

        this.cardCtrlList.Add(cardCtrl);
    }


}
