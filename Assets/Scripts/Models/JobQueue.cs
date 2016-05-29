using UnityEngine;
using System.Collections.Generic;
using System;


public class JobQueue
{
    private Queue<Job> jobQueue;

    private Action<Job> cbJobCreated;


    public JobQueue()
    {
        jobQueue = new Queue<Job>();
    }


    public void Enqueue(Job job)
    {
        jobQueue.Enqueue(job);

        if (cbJobCreated != null)
        {
            cbJobCreated(job);
        }
    }

    public Job Dequeue()
    {
        if (jobQueue.Count == 0)
        {
            return null;
        }
        return jobQueue.Dequeue();
    }

    public void RegisterJobCreationCallback(Action<Job> callback)
    {
        cbJobCreated += callback;
    }

    public void UnegisterJobCreationCallback(Action<Job> callback)
    {
        cbJobCreated -= callback;
    }
}
