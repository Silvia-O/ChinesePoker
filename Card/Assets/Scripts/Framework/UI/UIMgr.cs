using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : ManagerBase 
{

    public static UIMgr Instance = null;

    void Awake()
    {
        Instance = this;
    }
}
