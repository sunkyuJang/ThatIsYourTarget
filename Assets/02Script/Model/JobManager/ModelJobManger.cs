//using System;

//public class ModelJobManger : JobManager
//{
//    public enum State { CreatingAnimationPlayerJob, End, Non }
//    public AnimationPointHandler OriginalAPH { get; set; }
//    public Action ReservatedAction { get; set; }

//    public ModelJobManger(object section, Action endjob, AnimationPointHandler originalAPH, Action reservatedAction, ModelJob job) : base(section, endjob)
//    {
//        OriginalAPH = originalAPH;
//        ReservatedAction = reservatedAction;
//    }

//    public override void StartJob()
//    {
//        base.StartJob();
//    }

//    //ParsedQuery<Job> CreatingJob()
//    //{
//    //    for (State i = 0; i < State.Non; i++)
//    //    {
//    //        Job job = null;
//    //        switch (i)
//    //        {
//    //            case State.CreatingAnimationPlayerJob:

//    //                break;
//    //        }
//    //    }
//    //}


//    public class ModelJob : Job
//    {
//        public AnimationPointHandler aph { private set; get; }
//        public Action<AnimationPointHandler> returnAPH { private set; get; }
//        public ModelJob(JobManager jobManager, AnimationPointHandler aph, Action<AnimationPointHandler> returnAPH) : base(jobManager)
//        {
//            this.aph = aph;
//            this.returnAPH = returnAPH;
//        }
//    }
//}
