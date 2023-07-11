// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace JobManager
// {
//     public class JobManager
//     {
//         private IJobStarter jobStarter;
//         private Queue<Job> jobList;
//         private bool shouldCancle = false;
//         private object section = null;
//         public bool IsSameSection(object section) => this.section.Equals(section);
//         public JobManager(object section)
//         {
//             this.section = section;
//         }

//         public virtual void StartJob()
//         {
//             NextJob();
//         }
//         public virtual void NextJob()
//         {
//             if (shouldCancle || jobList.Count <= 0)
//             {
//                 EndJob();
//             }
//             else
//             {
//                 jobList.Dequeue().ProcesseJob();
//             }
//         }
//         public virtual void EndJob()
//         {
//             jobStarter.EndJob();
//         }
//         public virtual void CancleJob()
//         {
//             shouldCancle = true;
//         }
//         public virtual void AddJob(Job job)
//         {
//             jobList.Enqueue(job);
//         }
//         public Job CreatingJob<T>() where T : Job
//         {
//             return new Job(this);
//         }
//     }

//     public class Job
//     {
//         private JobManager jobManager;
//         public Action jobAction;
//         public Job(JobManager jobManager)
//         {
//             this.jobManager = jobManager;
//         }

//         public virtual void ProcesseJob()
//         {
//             jobAction?.Invoke();
//             if (jobAction == null) Debug.Log("jobAction is missing");

//             jobManager.NextJob();
//         }
//     }

//     public class ModelHandlerJob : Job
//     {
//         public ActionPoint ap { private set; get; }
//         public ActionPointHandler.WalkingState walkingState { private set; get; }
//         public ModelHandlerJob(JobManager jobManager, ActionPoint ap, ActionPointHandler.WalkingState walkingState) : base(jobManager)
//         {
//             this.ap = ap;
//             this.walkingState = walkingState;
//         }
//     }
// }


