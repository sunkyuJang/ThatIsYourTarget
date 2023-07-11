using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrepareStateModule
{
    public void SetPrepare(IPrepareModuleData data);
    public bool IsPrepared();
}

public interface IPrepareModuleData { }
