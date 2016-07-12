using Pathfinding;
using System;
using UnityEngine;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


public class Character : IXmlSerializable
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

	public void UnregisterOnChangedCallback(Action<Character> callback)
	{
		cbOnChanged -= callback;
	}


	private void UpdateSeekNewJob(float deltaTime)
	{
		if (currentJob == null)
		{
			currentJob = WorldController.WorldData.jobQueue.Dequeue();

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
		if (currentJob != null && CurrentTile == destinationTile)
		{
            currentJob.DoWork(deltaTime);
        }
	}

    private void UpdateMovement(float deltaTime)
    {
        if (CurrentTile == destinationTile)
        {
            AStarPath = null;
            return;
        }

        // NOTE:
        //      currTile = The tile I am currently in (and may be in the process of leaving)
        //      nextTile = The tile I am about to entering
        //      destTile = Our final destination -- we never enter this tile directly, but instead use it as a pathfinding 'target'
        // TODO: When we have a 'recruit' command, keep in mind that we may actually WANT to move directly to the destTile.

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
                // Let's ignore the first tile, because that's the tile we're currently in.
                AStarPath.Dequeue();
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
        float totalDistanceToTravel = Mathf.Sqrt(Mathf.Pow(CurrentTile.X - nextTile.X, 2) +
                                                    Mathf.Pow(CurrentTile.Y - nextTile.Y, 2));

        switch (nextTile.Enterable)
        {
            case Enterability.Never:
                {
                    // Most likely, a wall was just built and we need to reset our pathfinding information.
                    // TODO: Ideally, when a wall gets spawned, we should invalidate our path immediately,
                    //       so that we don't waste a bunch of time walking towards a dead end.
                    //       To save CPU, maybe we can only check every so often?
                    //       Or maybe we should register a callback to the OnTileChnaged event?
                    Debug.LogError("FIXME: A character tried to enter an unwalkable tile!");
                    nextTile = null;
                    AStarPath = null;
                    return;
                }
            case Enterability.Soon:
                {
                    // We can't enter the tile NOW, but we should be able to enter it in a moment, this is likely a door.
                    // So we don't bail on our movement/path, but we do return now and don't actually process the movement.
                    return;
                }
            case Enterability.Yes:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }

		float distanceThisFrame = speed / nextTile.MovementCost * deltaTime;
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


	#region Saving & Loading
	public XmlSchema GetSchema() { return null; }

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteStartElement("Character");
			writer.WriteAttributeString("X", CurrentTile.X.ToString());
			writer.WriteAttributeString("Y", CurrentTile.Y.ToString());
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader)
	{
		//MovementCost = float.Parse(reader.GetAttribute("MovementCost"));
	}
	#endregion
}
