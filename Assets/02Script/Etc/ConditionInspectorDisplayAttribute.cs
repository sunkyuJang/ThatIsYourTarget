using UnityEngine;
using UnityEditor;
public class ConditionShowing : PropertyAttribute
{
    public int conditionIndex;
    public ConditionShowing(int conditionIndex) => this.conditionIndex = conditionIndex;
}