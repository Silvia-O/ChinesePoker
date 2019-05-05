using MyProtocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : UIBase
{

    private void Awake()
    {
        Bind(UIEvent.SHOW_ENTER_ROOM_BUTTON);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_ENTER_ROOM_BUTTON:
                btnEnter.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private Button btnMatch;
    private Text txtDes;
    private Button btnCancel;
    private Button btnEnter;

    private SocketMsg socketMsg;

    // Use this for initialization
    void Start()
    {
        btnMatch = transform.Find("btnMatch").GetComponent<Button>();
        txtDes = transform.Find("txtDes").GetComponent<Text>();
        btnCancel = transform.Find("btnCancel").GetComponent<Button>();
        btnEnter = transform.Find("btnEnter").GetComponent<Button>();

        btnMatch.onClick.AddListener(MatchClick);
        btnCancel.onClick.AddListener(CancelClick);
        btnEnter.onClick.AddListener(EnterClick);

        socketMsg = new SocketMsg();

        // default hidden
        SetObjectsActive(false);
        btnEnter.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (txtDes.gameObject.activeInHierarchy == false)
            return;

        timer += Time.deltaTime;
        
        if (timer >= intervalTime)
        {
            doAnimation();
            timer = 0f;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        btnMatch.onClick.RemoveListener(MatchClick);
        btnCancel.onClick.RemoveListener(CancelClick);
        btnEnter.onClick.RemoveListener(EnterClick);
    }

    private void EnterClick()
    {
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(2, null));
    }

    private void MatchClick()
    {
        socketMsg.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        SetObjectsActive(true);

        // hidden active
        this.btnMatch.interactable = (false);
    }

    private void CancelClick()
    {
        socketMsg.Change(OpCode.MATCH, MatchCode.LEAVE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        SetObjectsActive(false);

        // hidden active
        this.btnMatch.interactable = true;
    }

    /// <summary>
    /// set active objects after clicking match button
    /// </summary>
    private void SetObjectsActive(bool active)
    {
        txtDes.gameObject.SetActive(active);
        btnCancel.gameObject.SetActive(active);
    }

    private string defaultText = "正在寻找房间";
    private int dotCount = 0;
    private float intervalTime = 1f;
    private float timer = 0f;

    private void doAnimation()
    {
        txtDes.text = defaultText;

        dotCount++;
        if (dotCount > 5)
            dotCount = 1;

        for (int i = 0; i < dotCount; i++)
        {
            txtDes.text += ".";
        }
    }

}
