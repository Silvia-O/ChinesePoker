using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBase
{
    /// <summary>
    /// message list
    /// </summary>
    private List<int> eventList = new List<int>();

    /// <summary>
    /// bind message
    /// </summary>
    /// <param name="eventCodes"></param>
    protected void Bind(params int[] eventCodes)
    {
        eventList.AddRange(eventCodes);
        UIMgr.Instance.Add(eventList.ToArray(), this);
    }

    /// <summary>
    /// unbind message
    /// </summary>
    protected void UnBind()
    {
        UIMgr.Instance.Remove(eventList.ToArray(), this);
        eventList.Clear();
    }

    /// <summary>
    /// automatically remove bound message
    /// </summary>
    public virtual void OnDestroy()
    {
        if (eventList != null)
            UnBind();
    }

    /// <summary>
    /// dispatch message
    /// </summary>
    /// <param name="areaCode"></param>
    /// <param name="eventCode"></param>
    /// <param name="message"></param>
    public void Dispatch(int areaCode, int eventCode, object message)
    {
        MsgCenter.Instance.Dispatch(areaCode, eventCode, message);
    }

    /// <summary>
    /// set panel active
    /// </summary>
    /// <param name="active"></param>
    protected void setPanelActive(bool active)
    {
        gameObject.SetActive(active);
    }

}
