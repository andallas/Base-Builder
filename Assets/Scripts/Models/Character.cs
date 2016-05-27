using UnityEngine;


public class Character
{
    public Tile CurrentTile { get; protected set; }

    private Tile destinationTile;
    private float movementPercentage;
    private float speed = 2f;


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
        if (CurrentTile == destinationTile)
        {
            return;
        }

        float totalDistanceToTravel = Mathf.Pow(CurrentTile.X - destinationTile.X, 2) + Mathf.Pow(CurrentTile.Y - destinationTile.Y, 2);
        float distanceThisFrame = speed * deltaTime;
        float percentageThisFrame = totalDistanceToTravel / distanceThisFrame;
        
        movementPercentage += percentageThisFrame;
        if (movementPercentage >= 1)
        {
            CurrentTile = destinationTile;
            movementPercentage = 0;

            // TODO: Do we want to retain any overshot movement? Or should we actually continue to zero out movementPercentage?
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
}
