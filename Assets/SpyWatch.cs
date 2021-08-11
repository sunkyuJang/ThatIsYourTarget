using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

public class SpyWatch : MonoBehaviour
{
    public GameObject WatchFollowPosition;
    public GameObject window;
    // private void Start()
    // {
    //     gameObject.layer = LayerMask.NameToLayer("Default");
    // }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Index2")
        {
            window.SetActive(!window.activeSelf);
        }
    }
}
