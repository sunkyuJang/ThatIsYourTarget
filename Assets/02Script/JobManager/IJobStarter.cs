using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJobStarter
{
    void StartJob(Job jobOption);
    void StopJob();
    bool IsSameSection(Job job);
}
