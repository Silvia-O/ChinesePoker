using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class of all the module manager 
/// </summary>
public class ManagerBase : MonoBase
{

    /// <summary>
    /// store event code with mono event
    /// </summary>
    private Dictionary<int,List<MonoBase>> dict = new Dictionary<int, List<MonoBase>>();

    /// <summary>
    /// manage message
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="message"></param>
    public override void Execute(int eventCode, object message)
    {
        if (!dict.ContainsKey(eventCode))
        {
            Debug.LogWarning("Not Registered: " + eventCode);
            return;
        }

        List<MonoBase> list = dict[eventCode];
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Execute(eventCode, message);
        }
    }
    
    /// <summary>
    /// add single event
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="mono"></param>
    public void Add(int eventCode, MonoBase mono)
    {
        List<MonoBase> list = null;

        // not registered before
        if (!dict.ContainsKey(eventCode))
        {
            list = new List<MonoBase>();
            list.Add(mono);
            dict.Add(eventCode, list);
            return;
        }

        // registered before
        list = dict[eventCode];
        list.Add(mono);
    }

    /// <summary>
    /// add multiple event
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="mono"></param>
    public void Add(int[] eventCodes, MonoBase mono)
    {
        for (int i = 0; i < eventCodes.Length; i++)
        {
            Add(eventCodes[i], mono);
        }
    }


    /// <summary>
    /// remove single event
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="mono"></param>
    public void Remove(int eventCode, MonoBase mono)
    {
        // not registered before
        if (!dict.ContainsKey(eventCode))
        {
            Debug.LogWarning("No Event: " + eventCode + "Can't be removed.");
            return;
        }

        List<MonoBase> list = dict[eventCode];

        if (list.Count == 1)
            dict.Remove(eventCode);
        else
            list.Remove(mono);
    }

    
    /// <summary>
    /// remove multiple event
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="mono"></param>
    public void Remove(int[] eventCodes, MonoBase mono)
    {
        for (int i = 0; i < eventCodes.Length; i++)
        {
            Remove(eventCodes[i], mono);
        }
    }

}
