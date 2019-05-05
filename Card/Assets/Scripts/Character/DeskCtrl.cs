using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskCtrl : CharacterBase
{
    private void Awake()
    {
        Bind(CharacterEvent.UPDATE_SHOW_DESK);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.UPDATE_SHOW_DESK:
                UpdateShowDesk(message as List<CardDto>);
                break;
            default:
                break;
        }
    }


    private List<CardCtrl> cardCtrlList;
    private Transform cardParent;

    // Use this for initialization
    void Start()
    {
        cardParent = transform.Find("CardPoint");
        cardCtrlList = new List<CardCtrl>();
    }

    /// <summary>
    /// update cards
    /// </summary>
    /// <param name="cardList"></param>
    private void UpdateShowDesk(List<CardDto> cardList)
    {
        //33 34567
        //34567  3

        if (cardCtrlList.Count > cardList.Count)
        {
            // original more than now
            int index = 0;
            foreach (var cardCtrl in cardCtrlList)
            {
                cardCtrl.gameObject.SetActive(true);
                cardCtrl.InitCards(cardList[index], index, true);
                index++;
                // no card
                if (index == cardList.Count)
                {
                    break;
                }
            }
            for (int i = index; i < cardCtrlList.Count; i++)
            {
                cardCtrlList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            // original more than now
            int index = 0;
            foreach (var cardCtrl in cardCtrlList)
            {
                cardCtrl.gameObject.SetActive(true);
                cardCtrl.InitCards(cardList[index], index, true);
                index++;
            }
            // recreate 
            GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");
            for (int i = index; i < cardList.Count; i++)
            {
                CreateGo(cardList[i], i, cardPrefab);
            }
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
        cardGo.transform.localPosition = new Vector2((0.3f * index), 0);
        CardCtrl cardCtrl = cardGo.GetComponent<CardCtrl>();
        cardCtrl.InitCards(card, index, true);

        // save
        this.cardCtrlList.Add(cardCtrl);
    }

}
