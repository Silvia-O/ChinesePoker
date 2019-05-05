using MyProtocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.REFRESH_INFO_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.REFRESH_INFO_PANEL:
                UserDto user = message as UserDto;
                RefreshPanel(user.Name, user.Lv, user.Exp, user.Bean);
                break;
            default:
                break;
        }
    }

    //private Image imgHead;
    private Text txtName;
    private Text txtLv;
    private Slider sldExp;
    private Text txtExp;
    private Text txtBean;

    // Use this for initialization
    void Start()
    {
        txtName = transform.Find("txtName").GetComponent<Text>();
        txtLv = transform.Find("txtLv").GetComponent<Text>();
        sldExp = transform.Find("sldExp").GetComponent<Slider>();
        txtExp = transform.Find("txtExp").GetComponent<Text>();
        txtBean = transform.Find("txtBean").GetComponent<Text>();
    }

    
    private void RefreshPanel(string name, int lv, int exp, int bean)
    {
        txtName.text = name;
        txtLv.text = "Lv." + lv;
        txtExp.text = exp + " / " + lv * 100; 
        sldExp.value = (float)exp / (lv * 100);
        txtBean.text = "× " + bean.ToString();
    }

}
