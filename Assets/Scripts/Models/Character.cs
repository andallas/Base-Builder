using UnityEngine;
using System;


public class Character
{
    public Tile CurrentTile { get; protected set; }

    private Tile destinationTile;
    private float movementPercentage;
    private float speed = 2f;
    private Action<Character> cbOnChanged;
    private Job currentJob;

    public float X
    {
        get
        {
            return Mathf.Lerp(CurrentTile.X, destinationTile.X, movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(CurrentTile.Y, destinationTile.Y, movementPercentage);
        }
    }


    public Character(Tile tile)
    {
        CurrentTile = destinationTile = tile;
    }


    public void Update(float deltaTime)
    {
        // Seek new job
        if (currentJob == null)
        {
            currentJob = CurrentTile.World.jobQueue.Dequeue();

            if (currentJob != null)
            {
                destinationTile = currentJob.Tile;
                currentJob.RegisterJobCancelCallback(OnJobEnded);
                currentJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        // Work on job or move to job
        if (CurrentTile == destinationTile)
        {
            if (currentJob != null)
            {
                currentJob.DoWork(deltaTime);
            }
        }
        else
        {
            UpdateMovement(deltaTime);
        }
       
        if (cbOnChanged != null)
        {
            cbOnChanged(this);
        }
    }
    

    public void SetDestination(Tile tile)
    {
        if (!CurrentTile.IsNeighbor(tile))
        {
            Debug.LogWarning("SetDestination - Destination tile is not a neighbor.");
        }

        destinationTile = tile;
    }

    public void RegisterOnChangedCallback(Action<Character> callback)
    {
        cbOnChanged += callback;
    }

    public void UnregisterONChangedCallback(Action<Character> callback)
    {
        cbOnChanged -= callback;
    }


    private void UpdateMovement(float deltaTime)
    {
        float totalDistanceToTravel = Mathf.Sqrt(   Mathf.Pow(CurrentTile.X - destinationTile.X, 2) +
                                                    Mathf.Pow(CurrentTile.Y - destinationTile.Y, 2));

        float distanceThisFrame = speed * deltaTime;
        float percentageThisFrame = distanceThisFrame / totalDistanceToTravel;

        // TODO: Maybe we can LERP this to smooth it out, or use some kind of curve value.
        movementPercentage += percentageThisFrame;
        if (movementPercentage >= 1)
        {
            CurrentTile = destinationTile;
            movementPercentage = 0;

            // TODO: Do we want to retain any overshot movement? Or should we actually continue to zero out movementPercentage?
        }
    }

    private void OnJobEnded(Job job)
    {
        if (job != currentJob)
        {
            Debug.LogError("OnJobEnded - Character being told about job that doesn't belong to character. Remember to unregister callbacks!");
            return;
        }

        currentJob = null;
    }
}
