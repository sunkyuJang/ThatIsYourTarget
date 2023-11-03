using Unity.VisualScripting;
using UnityEngine;

public class ModelAnimationPlayer : IJobStarter<ModelAPHJobManger.ModelJob>
{
    public Model Model { private set; get; }
    AnimationPointHandler ActionPointHandler { set; get; }
    NaviController NaviController { set; get; }
    AniController AniController { set; get; }
    ModelAPHJobManger.ModelJob ModelJob { set; get; }
    ModelAnimationPlayerJobManager JobManager { set; get; }

    public ModelAnimationPlayer(Model model, Transform actorTransform)
    {
        Model = model;
        NaviController = actorTransform.GetComponent<NaviController>();
        AniController = actorTransform.GetComponent<AniController>();

        if (NaviController == null
            && AniController == null) { Debug.Log("some of component in actor is missing"); }
    }
    public void StartJob(ModelAPHJobManger.ModelJob job)
    {
        if (ActionPointHandler != null)
        {
            ModelJob?.returnAPH(ActionPointHandler);
        }

        if (JobManager != null)
            JobManager.CancleJob();

        StopJob();

        ModelJob = job;
        ActionPointHandler = ModelJob.aph;
        JobManager = new ModelAnimationPlayerJobManager(
                        runAfterJobEnd: () => { ModelJob.EndJob(); },
                        modelJob: ModelJob,
                        naviJobStarter: NaviController,
                        aniJobstarter: AniController);
        JobManager.StartJob();
    }

    public void StopJob()
    {
        AniController.StopJob();
        NaviController.StopJob();
    }
}