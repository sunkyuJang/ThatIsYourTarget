using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadCasterForPerson : MonoBehaviour
{
    [HideInInspector]
    public static BroadCasterForPerson instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void BroadCastWithNotice(Vector3 centerWorldPosition, float radius, float absoluteDist)
    {
        var touchedPersonGroup = GetPersonInSoundCast(centerWorldPosition, radius, absoluteDist);

    }

    List<List<RaycastHit>> GetPersonInSoundCast(Vector3 centerWorldPosition, float radius, float absoluteDist)
    {
        var touchedPersons = Physics.SphereCastAll(centerWorldPosition, radius, Vector3.zero, 0f, 1 << LayerMask.NameToLayer("Person"));

        var yUnit = 1f;
        var groupList = new List<List<RaycastHit>>();

        for (int i = 0; i < touchedPersons.Length; i++)
        {
            var p = touchedPersons[i];
            var pPosition = p.transform.position;
            if (Vector3.Distance(centerWorldPosition, pPosition) < absoluteDist)
            {
                //Go To Alert.Attack All of in List
            }
            else
            {
                var dir = pPosition - centerWorldPosition;
                var other = Physics.RaycastAll(centerWorldPosition, dir, Vector3.Distance(centerWorldPosition, p.transform.position));

                if (CanSkipObstacle(other, p, centerWorldPosition, absoluteDist))
                {
                    var positionY = p.transform.position.y;
                    if (positionY > positionY + yUnit)
                        groupList[0].Add(p);
                    else if (positionY < positionY - yUnit)
                        groupList[2].Add(p);
                    else
                        groupList[1].Add(p);
                }
            }
        }

        return groupList;
    }

    bool CanSkipObstacle(RaycastHit[] hits, RaycastHit p, Vector3 centerWorldPosition, float absoluteDist)
    {
        var counter = 0;
        for (int j = 0; j < hits.Length; j++)
        {
            // skip it self && dist must be longer than absoluteDist 
            if (hits[j].transform != p.transform && Vector3.Distance(hits[j].transform.position, centerWorldPosition) > absoluteDist)
            {
                counter++;
                if (counter > 2)
                {
                    return false;
                }
            }
        }

        return true;
    }

    class BroadCasterEntity
    {
        public List<Person> persons = new List<Person>();
    }
}
