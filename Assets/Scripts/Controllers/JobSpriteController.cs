using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class JobSpriteController : MonoBehaviour
{
    // TODO: This bare-bones controller is mostly just going to piggyback on
    //       FurnitureSpriteController because we don't yet fully know what
    //       our job system is going to look like in the end.

    private FurnitureSpriteController fsc;
    private Dictionary<Job, GameObject> jobGameObjectMap;


    void Start()
    {
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
        jobGameObjectMap = new Dictionary<Job, GameObject>();

        WorldController.WorldData.jobQueue.RegisterJobCreationCallback(OnJobCreated);
    }


    private void OnJobCreated(Job job)
    {
        // TODO: We can only do furniture-building jobs.

        int x = job.Tile.X;
        int y = job.Tile.Y;

        GameObject job_go = new GameObject();
        job_go.name = "JOB_" + job.JobObjectType + "_" + x + "_" + y;
        job_go.transform.position = new Vector3(x, y, 0);
        job_go.transform.SetParent(transform, true);

        SpriteRenderer job_sr = job_go.AddComponent<SpriteRenderer>();
        job_sr.sprite = fsc.GetSpriteForFurniture(job.JobObjectType);
        job_sr.color = new Color(0.5f, 1f, 0.5f, 0.25f);

        jobGameObjectMap.Add(job, job_go);

        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);
    }

    private void OnJobEnded(Job job)
    {
        // TODO: We can only do furniture-building jobs.
        job.UnregisterJobCompleteCallback(OnJobEnded);
        job.UnregisterJobCancelCallback(OnJobEnded);

        GameObject job_go = jobGameObjectMap[job];
        Destroy(job_go);
    }
}
