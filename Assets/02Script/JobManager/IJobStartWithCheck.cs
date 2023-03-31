using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJobStartWithCheck : IJobStarter
{
    bool IsSameSection(Job job);
}
