using UnityEngine;

public class ModelAnimationPlayer : IJobStarter<ModelAPHJobManger.ModelJob>, IDamageController
{
    public Model Model { private set; get; }
    AnimationPointHandler actionPointHandler { set; get; }
    NaviController naviController { set; get; }
    AniController aniController { set; get; }
    RagDollHandler ragDollHandler { set; get; }
    ModelAPHJobManger.ModelJob modelJob { set; get; }
    ModelAnimationPlayerJobManager jobManager { set; get; }

    public ModelAnimationPlayer(Model model, Transform actorTransform)
    {
        Model = model;
        naviController = actorTransform.GetComponent<NaviController>();
        aniController = actorTransform.GetComponent<AniController>();
        ragDollHandler = actorTransform.GetComponent<RagDollHandler>();

        if (naviController == null
            && aniController == null) { Debug.Log("some of component in actor is missing"); }
    }
    public void StartJob(ModelAPHJobManger.ModelJob job)
    {
        if (actionPointHandler != null)
        {
            modelJob?.returnAPH(actionPointHandler);
        }

        if (jobManager != null)
            jobManager.CancleJob();

        StopJob();

        modelJob = job;
        actionPointHandler = modelJob.aph;
        jobManager = new ModelAnimationPlayerJobManager(
                        runAfterJobEnd: () => { modelJob.EndJob(); },
                        modelJob: modelJob,
                        naviJobStarter: naviController,
                        aniJobstarter: aniController);
        jobManager.StartJob();
    }

    public void StopJob()
    {
        aniController.StopJob();
        naviController.StopJob();
    }

    public void SetDead()
    {
        StopJob();
        ragDollHandler.TrunOnRigid(true);
    }

    public bool SetDamage(float damege)
    {
        return ((IDamageController)Model).SetDamage(damege);
    }

}