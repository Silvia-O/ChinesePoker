using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBase
{
    /// <summary>
    /// message list
    /// </summary>
    private List<int> list = new List<int>();

    /// <summary>
    /// bind message
    /// </summary>
    /// <param name="eventCodes">Event codes.</param>
    protected void Bind(params int[] eventCodes)
    {
        list.AddRange(eventCodes);
        CharacterMgr.Instance.Add(list.ToArray(), this);
    }

    /// <summary>
    /// unbind message
    /// </summary>
    protected void UnBind()
    {
        CharacterMgr.Instance.Remove(list.ToArray(), this);
        list.Clear();
    }

    /// <summary>
    /// automatically remove bound message
    /// </summary>
    public  virtual void OnDestroy()
    {
        if (list != null)
            UnBind();
    }

    /// <summary>
    /// dispatch message
    /// </summary>
    /// <param name="areaCode">Area code.</param>
    /// <param name="eventCode">Event code.</param>
    /// <param name="message">Message.</param>
    public void Dispatch(int areaCode, int eventCode, object message)
    {
        MsgCenter.Instance.Dispatch(areaCode, eventCode, message);
    }

}