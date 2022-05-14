using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Model
{
    public const string playerTag = "Player";
    public Renderer renderer;
    public Material belongTo { get { return renderer.material; } }
}
