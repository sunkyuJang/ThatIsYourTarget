using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolerConnector
{
    public void WhenRetrieveFromPooler();
    public void WhenStoreToPooler();
}
