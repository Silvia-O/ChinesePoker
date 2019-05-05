using System.Collections;
using System.Collections.Generic;
using MyProtocol.Dto.Fight;
using UnityEngine;
using UnityEngine.UI;

public class UpPanel : UIBase
{
    void Awake()
    {
        Bind(UIEvent.SET_TABLE_CARDS);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SET_TABLE_CARDS:
                setTableCards(message as List<CardDto>);
                break;
            default:
                break;
        }
    }
    
    private Image[] imgCards = null;
    
    // Start is called before the first frame update
    void Start()
    {
        imgCards = new Image[3];
        imgCards[0] = transform.Find("imgCard1").GetComponent<Image>();
        imgCards[1] = transform.Find("imgCard2").GetComponent<Image>();
        imgCards[2] = transform.Find("imgCard3").GetComponent<Image>();
    }
    
    private void setTableCards(List<CardDto> cards)
    {
        imgCards[0].sprite = Resources.Load<Sprite>("Poker/" + cards[0].Name);
        imgCards[1].sprite = Resources.Load<Sprite>("Poker/" + cards[1].Name);
        imgCards[2].sprite = Resources.Load<Sprite>("Poker/" + cards[2].Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
