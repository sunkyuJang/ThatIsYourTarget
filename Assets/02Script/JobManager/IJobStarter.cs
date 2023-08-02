public interface IJobStarter<T> where T : Job
{
    void StartJob(T jobOption);
    void StopJob();
}
