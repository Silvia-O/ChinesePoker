using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMgr : ManagerBase {

    public static CharacterMgr Instance = null;

    void Awake()
    {
        Instance = this;
    }

}
