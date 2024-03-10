using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHandler : MonoBehaviour
{
    public List<Human> Humans { set; get; } = new List<Human>();
    public List<Material> belongKinds = new List<Material>();
    Material TargetColor { get { return belongKinds[0]; } }
    int NormalPersonStartFrom { get { return 1; } }
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var human = transform.GetChild(i).GetComponent<Human>();
            //person.SetBelongTo(belongKinds[Random.Range(NormalPersonStartFrom, belongKinds.Count)]);
            Humans.Add(human);
        }
    }

    public List<Human> SetEnemy(int enemyCount)
    {
        var nowHumanList = new List<Human>();
        for (int i = 0; i < enemyCount; i++)
        {
            var index = Random.Range(0, Humans.Count);
            var nowHuman = Humans[index];
            Humans.RemoveAt(index);
            nowHuman.belongTo = TargetColor;
            nowHumanList.Add(nowHuman);
        }
        Humans.AddRange(nowHumanList);

        return nowHumanList;
    }
}
