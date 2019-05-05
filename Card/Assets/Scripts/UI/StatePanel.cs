using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyProtocol.Dto;
using MyProtocol.Dto.Fight;

public class StatePanel : UIBase
{
    //fix bug
    //private void Awake()
    protected virtual void Awake()
    {
        Bind(UIEvent.PLAYER_HIDE_STATE);
        Bind(UIEvent.PLAYER_READY);
        Bind(UIEvent.PLAYER_LEAVE);
        Bind(UIEvent.PLAYER_ENTER);
        Bind(UIEvent.PLAYER_CHANGE_IDENTITY);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.PLAYER_HIDE_STATE:
                {
                    txtReady.gameObject.SetActive(false);
                }
                break;
            case UIEvent.PLAYER_READY:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        ReadyState();
                    break;
                }
            case UIEvent.PLAYER_LEAVE:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        setPanelActive(false);
                    break;
                }
            case UIEvent.PLAYER_ENTER:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        setPanelActive(true);
                    break;
                }
            case UIEvent.PLAYER_CHANGE_IDENTITY:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        SetIdentity(1);
                    break;
                }
            default:
                break;
        }
    }


    protected UserDto userDto;

    protected Image imgIdentity;
    protected Text txtReady;
    protected virtual void Start()
    {
        imgIdentity = transform.Find("imgIdentity").GetComponent<Image>();
        txtReady = transform.Find("txtReady").GetComponent<Text>();

        // defualt
        txtReady.gameObject.SetActive(false);
        SetIdentity(0);
    }

    protected virtual void ReadyState()
    {
        txtReady.gameObject.SetActive(true);
    }

    /// <summary>
    /// set player identity
    /// 0-farmer 1-landlord
    /// </summary>
    protected void SetIdentity(int identity)
    {
        if (identity == 0)
        {
            imgIdentity.sprite = Resources.Load<Sprite>("Identity/Farmer");
        }
        else if (identity == 1)
        {
            imgIdentity.sprite = Resources.Load<Sprite>("Identity/Landlord");
        }
    }

 
    protected int showTime = 3;

    protected float timer = 0f;
 
    private bool isShow = false;

    protected virtual void Update()
    {
        if (isShow == true)
        {
            timer += Time.deltaTime;
            if (timer >= showTime)
            {
                timer = 0f;
                isShow = false;
            }
        }
    }


}
