using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftPlayerCtrl : CharacterBase
{
    private void Awake()
    {
        Bind(CharacterEvent.INIT_LEFT_CARD,
            CharacterEvent.ADD_LEFT_CARD,
            CharacterEvent.REMOVE_LEFT_CARD);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.INIT_LEFT_CARD:
                StartCoroutine(InitCardList());
                break;
            case CharacterEvent.ADD_LEFT_CARD:
                AddTableCard();
                break;
            case CharacterEvent.REMOVE_LEFT_CARD:
                RemoveCard((message as List<CardDto>).Count);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// remove card
    /// </summary>
    private void RemoveCard( int cardCount )
    {
        // ***** ******
        for (int i = cardCount; i < cardObjectList.Count; i++)
        {
            cardObjectList[i].SetActive(false);
        }
    }

    /// <summary>
    /// add table cards
    /// </summary>
    /// <param name="cardList"></param>
    private void AddTableCard()
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/OtherCard");
        for (int i = 0; i < 3; i++)
        {
            CreateGo(i, cardPrefab);
        }
    }

    private List<GameObject> cardObjectList;


    private Transform cardParent;

    // Use this for initialization
    void Start()
    {
        cardParent = transform.Find("CardPoint");
        cardObjectList = new List<GameObject>();
    }

    /// <summary>
    /// init card list
    /// </summary>
    private IEnumerator InitCardList()
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/OtherCard");

        for (int i = 0; i < 17; i++)
        {
            CreateGo(i, cardPrefab);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// create cards game object
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cardPrefab"></param>
    private void CreateGo(int index, GameObject cardPrefab)
    {
        GameObject cardGo = Object.Instantiate(cardPrefab, cardParent) as GameObject;
        cardGo.transform.localPosition = new Vector2((0.2f * index), 0);
        cardGo.GetComponent<SpriteRenderer>().sortingOrder = index;

        this.cardObjectList.Add(cardGo);
    }
}
