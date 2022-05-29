using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const string playerTag = "Player";
    [SerializeField]
    private Renderer renderer;
    public Material belongTo { get { return renderer.material; } }
}
