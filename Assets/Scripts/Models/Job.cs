﻿using System;


public class Job
{
    // This class holds info for a queued up job, which can include
    // things like placing character, moving stored inventory,
    // working at a desk, and maybe even fighting enemies.

    public Tile Tile { get; protected set; }
    private float jobTime;

    public string JobObjectType { get; protected set; }


    private Action<Job> cbJobComplete;
    private Action<Job> cbJobCancel;


    public Job(Tile tile, string jobObjectType, Action<Job> cbJobComplete, float jobTime = 0.1f)
    {
        Tile = tile;
        JobObjectType = jobObjectType;
        this.jobTime = jobTime;
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

    public void UnregisterJobCompleteCallback(Action<Job> callback)
    {
        cbJobComplete -= callback;
    }

    public void UnregisterJobCancelCallback(Action<Job> callback)
    {
        cbJobCancel -= callback;
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
