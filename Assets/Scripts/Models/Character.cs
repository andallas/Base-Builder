using UnityEngine;
using System;
using Pathfinding;


public class Character
{
    public Tile CurrentTile { get; protected set; }

    private Tile destinationTile;
    private Tile nextTile;
    private AStar AStarPath;
    private float movementPercentage;
    private float speed = 2f;
    private Action<Character> cbOnChanged;
    private Job currentJob;

    public float X
    {
        get
        {
            return Mathf.Lerp(CurrentTile.X, nextTile.X, movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(CurrentTile.Y, nextTile.Y, movementPercentage);
        }
    }


    public Character(Tile tile)
    {
        CurrentTile = destinationTile = nextTile = tile;
    }


    public void Update(float deltaTime)
    {
        UpdateSeekNewJob(deltaTime);
        UpdateDoJob(deltaTime);
        UpdateMovement(deltaTime);

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


    private void UpdateSeekNewJob(float deltaTime)
    {
        if (currentJob == null)
        {
            currentJob = CurrentTile.World.jobQueue.Dequeue();

            if (currentJob != null)
            {
                // TODO: Check to see if job is reachable, if not then place it on bottom of queue
                destinationTile = currentJob.Tile;
                currentJob.RegisterJobCancelCallback(OnJobEnded);
                currentJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }
    }

    private void UpdateDoJob(float deltaTime)
    {
        if (CurrentTile == destinationTile)
        //if (AStarPath != null && AStarPath.Length == 1)
        {
            if (currentJob != null)
            {
                currentJob.DoWork(deltaTime);
            }
        }
    }

    private void UpdateMovement(float deltaTime)
    {
        if (CurrentTile == destinationTile)
        {
            AStarPath = null;
            return;
        }

        // Get the next tile from our pathfinder if we need a new tile
        if (nextTile == null || nextTile == CurrentTile)
        {
            if (AStarPath == null || AStarPath.Length == 0)
            {
                AStarPath = new AStar(WorldController.WorldData, CurrentTile, destinationTile);
                if (AStarPath.Length == 0)
                {
                    Debug.LogError("UpdateMovement: AStar returned no path to destination!");
                    AStarPath = null;
                    AbandonJob();
                    return;
                }
            }

            nextTile = AStarPath.Dequeue();

            if (nextTile == CurrentTile)
            {
                Debug.LogError("UpdateMovement: Next tile is current tile?");
            }
        }

        //if (AStarPath != null && AStarPath.Length == 1)
        //{
        //    return;
        //}

        // Travel to the next tile
        float totalDistanceToTravel = Mathf.Sqrt(   Mathf.Pow(CurrentTile.X - nextTile.X, 2) +
                                                    Mathf.Pow(CurrentTile.Y - nextTile.Y, 2));

        float distanceThisFrame = speed * deltaTime;
        float percentageThisFrame = distanceThisFrame / totalDistanceToTravel;

        // TODO: Maybe we can LERP this to smooth it out, or use some kind of curve value.
        movementPercentage += percentageThisFrame;
        if (movementPercentage >= 1)
        {
            // TODO: Do we want to retain any overshot movement? Or should we actually continue to zero out movementPercentage?
            movementPercentage = 0;

            CurrentTile = nextTile;
        }
    }

    private void AbandonJob()
    {
        nextTile = destinationTile = CurrentTile;
        AStarPath = null;
        WorldController.WorldData.jobQueue.Enqueue(currentJob);
        currentJob = null;
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
