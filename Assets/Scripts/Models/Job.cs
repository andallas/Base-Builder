using UnityEngine;
using System;


public class Job
{
    // This class holds info for a queued up job, which can include
    // things like placing furniture, moving stored inventory,
    // working at a desk, and maybe even fighting enemies.

    public Tile Tile { get; protected set; }
    private float jobTime;


    private Action<Job> cbJobComplete;
    private Action<Job> cbJobCancel;


    public Job(Tile tile, Action<Job> cbJobComplete, float jobTime = 1f)
    {
        Tile = tile;
        RegisterJobCompleteCallback(cbJobComplete);
    }


    public void RegisterJobCompleteCallback(Action<Job> callback)
    {
        cbJobComplete += callback;
    }

    public void RegisterJobCancelCallback(Action<Job> callback)
    {
        cbJobCancel += callback;
    }

    public void DoWork(float workTime)
    {
        jobTime -= workTime;

        if (jobTime <= 0)
        {
            if (cbJobComplete != null)
            {
                cbJobComplete(this);
            }
        }
    }

    public void CancelJob()
    {
        if (cbJobCancel != null)
        {
            cbJobCancel(this);
        }
    }
}
