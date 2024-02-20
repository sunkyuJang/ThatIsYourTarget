using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
public class HumanAttackConditionHandler : AttackConditionerHandler
{
    public HumanAttackConditionHandler(AnimatorController animatorController) : base(animatorController) { }
    protected override List<string> GetAniNodeName()
    {
        List<string> enumDict = new List<string>();

        foreach (var enumValue in Enum.GetValues(typeof(StateNodeName)))
        {
            enumDict.Add(enumValue.ToString());
        }

        return enumDict;
    }

    //public 

    public override AnimationStateNode GetInitiatedNode(RequireData requireData)
    {
        AnimationStateNode node = null;
        var weapon = (requireData as PersonRequireData).Weapon;
        if (weapon == null)
        {
            Debug.Log("Somthing wrong when get initiateAPState");
            return null;
        }

        node = AttackingAniStateManager.GetStartNode();
        node = FindNextNode(requireData, node, null);
        return node;
    }

    protected override AnimationStateNode FindNextNode(RequireData requireData, AnimationStateNode node, Action<Animator> parametterSetter)
    {
        var personRequireData = requireData as PersonRequireData;
        var beforeNode = node;
        var afterNode = node;
        do
        {
            var find = false;
            var falseCount = 0;
            for (StateNode i = 0; i <= StateNode.Default; i++, falseCount++)
            {
                switch (i)
                {
                    case StateNode.Initiate: find = IsFindNextNodeNameFromStart(personRequireData, afterNode, ref parametterSetter, out afterNode); break;
                    case StateNode.Custom: find = IsFindNextNodeNameFromCustom(personRequireData, afterNode, ref parametterSetter, out afterNode); break;
                    case StateNode.Loop: find = IsFindNextNodeNameFromLoop(personRequireData, afterNode, ref parametterSetter, out afterNode); break;
                }

                if (find) break;
            }

            if (falseCount >= (int)StateNode.Default) return null;

            if (beforeNode.nowAnimation == afterNode.nowAnimation)
            {
                if (afterNode.isSubState)
                {
                    Debug.Log("Dont use loop transition with sub-State : " + afterNode.nowAnimation);
                }

                break;
            }


            //afterNode will play frist cause this condition  
        } while (afterNode.isSubState);

        if (afterNode != null)
            afterNode.WhenReadAPForSetParametter += parametterSetter;

        return afterNode;
    }


    protected bool IsFindNextNodeNameFromStart(PersonRequireData data, AnimationStateNode node, ref Action<Animator> action, out AnimationStateNode nextNode)
    {
        //initiate node
        nextNode = node;
        var weapon = data.Weapon;
        if (node.nowAnimation != StateNodeName.AttackingWeapon.ToString()) return false;

        for (int i = 0; i < node.nextAnimations.Count; i++)
        {
            //var x = node.nextAnimations[i];
            var shouldAdd = false;
            // if (x == StateNodeName.FistAttack.ToString())
            // {
            //     shouldAdd = weapon.GetWeaponType == Weapon.WeaponType.Fist;
            // }
            // else if (x == StateNodeName.HandGunAttack.ToString())
            // {
            //     shouldAdd = weapon.GetWeaponType == Weapon.WeaponType.HandGun;
            // }
            // else if (x == StateNodeName.RifleAttack.ToString())
            // {
            //     shouldAdd = weapon.GetWeaponType == Weapon.WeaponType.Rifle;
            // }
            // else if (x == StateNodeName.StickAttack.ToString())
            // {
            //     shouldAdd = weapon.GetWeaponType == Weapon.WeaponType.Stick;
            // };

            if (shouldAdd)
            {
                action += (Animator animator) => { animator.SetInteger(node.nowAnimation, i); };
                //nextNode = AttackingAniStateManager.GetStateNode(node.nextAnimations[i]);
                return true;
            }
        }

        return false;
    }

    bool IsFindNextNodeNameFromCustom(PersonRequireData data, AnimationStateNode node, ref Action<Animator> actions, out AnimationStateNode nextNode)
    {
        nextNode = node;
        return false;
    }

    bool IsFindNextNodeNameFromLoop(PersonRequireData data, AnimationStateNode node, ref Action<Animator> action, out AnimationStateNode nextNode)
    {
        nextNode = node;
        if (node.nextAnimations == null) return false;

        for (int i = 0; i < node.nextAnimations.Count; i++)
        {
            // var nextAnimation = node.nextAnimations[i];
            // if (nextAnimation == StateNodeName.FistAttack_Normal.ToString()
            // || nextAnimation == StateNodeName.HandGunAttack_Normal.ToString()
            // || nextAnimation == StateNodeName.RifleAttack_Normal.ToString()
            // || nextAnimation == StateNodeName.StickAttack_Normal.ToString())
            // {
            //     // var canAttack = data.Weapon.CanAttack(out Weapon.CanAttackStateError attackStateError);
            //     // if (!canAttack) return false;

            //     // action += (Animator animator) => { animator.SetInteger(nextAnimation, i); };
            //     // nextNode = AttackingAniStateManager.GetStateNode(node.nextAnimations[i]);
            //     // var random = UnityEngine.Random.Range(1, data.Weapon.LeftHitCount);
            //     // nextNode.maxLoop = random;

            //     return true;
            // }
        }

        return false;
    }

    int SetFunc(AnimationStateNode node)
    {
        return 0;
    }

    public enum StateNode
    {
        Initiate,
        Loop,
        Custom,
        Default,
    }

    public enum StateNodeName
    {
        // initiate
        AttackingWeapon,

        // eachState
        FistAttack,
        HandGunAttack,
        RifleAttack,
        StickAttack,

        // normal attack
        FistAttack_Normal,
        HandGunAttack_Normal,
        RifleAttack_Normal,
        StickAttack_Normal,

        //
    }

    public class PersonRequireData : RequireData
    {
        public Weapon Weapon { set; get; }

        public PersonRequireData(Weapon weapon)
        {
            Weapon = weapon;
        }
    }
}