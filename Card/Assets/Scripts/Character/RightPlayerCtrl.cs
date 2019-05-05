using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPlayerCtrl : CharacterBase
{
    private void Awake()
    {
        Bind(CharacterEvent.INIT_RIGHT_CARD,
            CharacterEvent.ADD_RIGHT_CARD,
            CharacterEvent.REMOVE_RIGHT_CARD);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.INIT_RIGHT_CARD:
                StartCoroutine(InitCardList());
                break;
            case CharacterEvent.ADD_RIGHT_CARD:
                AddTableCard();
                break;
            case CharacterEvent.REMOVE_RIGHT_CARD:
                RemoveCard((message as List<CardDto>).Count);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// add table card
    /// </summary>
    private void AddTableCard()
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/OtherCard");
        for (int i = 0; i < 3; i++)
        {
            CreateGo(i, cardPrefab);
        }
    }


    /// <summary>
    /// remove card game object
    /// </summary>
    /// <param name="cardCount"></param>
    private void RemoveCard(int cardCount)
    {
        // ***** ******
        for (int i = cardCount; i < cardObjectList.Count; i++)
        {
            cardObjectList[i].SetActive(false);
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
    /// create card game object
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cardPrefab"></param>
    private void CreateGo(int index, GameObject cardPrefab)
    {
        GameObject cardGo = Object.Instantiate(cardPrefab, cardParent) as GameObject;
        cardGo.transform.localPosition = new Vector2((0.15f * index), 0);
        cardGo.GetComponent<SpriteRenderer>().sortingOrder = index;
    }
}
