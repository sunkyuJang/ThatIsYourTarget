using System;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsHandler
{
    public IDamagePasser damagePasser { set; get; }
    public RagDollHandler RagDollHandler { private set; get; }
    public DamageContorller DamageContorller { private set; get; }
    public PhysicsHandler(Transform actor, IDamagePasser damagePasser)
    {
        var rigidbodies = actor.GetComponentsInChildren<Rigidbody>();
        //RagDollHandler = new RagDollHandler(rigidbodies);
        DamageContorller = new DamageContorller(damagePasser, actor);
    }
}