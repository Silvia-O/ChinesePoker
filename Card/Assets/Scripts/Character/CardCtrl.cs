using MyProtocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCtrl : MonoBehaviour
{
    public CardDto CardDto { get; private set; }
    
    // whether the card is selected
    public bool Selected { get; set; }

    private SpriteRenderer spriteRenderer;
    private bool isMine;

    /// <summary>
    /// init card data
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    /// <param name="isMine"></param>
    public void InitCards(CardDto card, int index, bool isMine)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.CardDto = card;
        this.isMine = isMine;
        
        if (Selected == true)
        {
            Selected = false;
            transform.localPosition -= new Vector3(0, 0.2f, 0);
        }
        string resPath = string.Empty;
        if (isMine)
        {
            resPath = "Poker/" + card.Name;
        }
        else
        {
            resPath = "Poker/CardBack";
        }
        Sprite sp = Resources.Load<Sprite>(resPath);
        spriteRenderer.sprite = sp;
        spriteRenderer.sortingOrder = index;
    }

    /// <summary>
    /// call when clicking the card
    /// </summary>
    private void OnMouseDown()
    {
        if (isMine == false)
            return;

        this.Selected = !Selected;
        if(Selected == true)
        {
            transform.localPosition += new Vector3(0, 0.2f, 0);
        }
        else
        {
            transform.localPosition -= new Vector3(0, 0.2f, 0);
        }
    }

    /// <summary>
    /// change card state after selecting
    /// </summary>
    public void SelectState()
    {
        if(Selected == false)
        {
            this.Selected = true;
            transform.localPosition += new Vector3(0, 0.2f, 0);
        }
    }
}
