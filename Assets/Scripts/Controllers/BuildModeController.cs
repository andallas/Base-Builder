using UnityEngine;


public class BuildModeController : MonoBehaviour
{
    private TileType _buildModeTile = TileType.Floor;
    private string _buildModeFurnitureType;
    private bool _isBuildModeFurniture = false;


    public void SetMode_BuildFurniture(string furnitureType)
    {
        _isBuildModeFurniture = true;
        _buildModeFurnitureType = furnitureType;
    }

    public void SetMode_BuildFloor()
    {
        _isBuildModeFurniture = false;
        _buildModeTile = TileType.Floor;
    }

    public void SetMode_Bulldoze()
    {
        _isBuildModeFurniture = false;
        _buildModeTile = TileType.Empty;
    }

    public void DoBuild(Tile tile)
    {
        if (_isBuildModeFurniture)
        {
            string furnitureType = _buildModeFurnitureType;
            
            World world = WorldController.WorldData;

            if (world.IsFurniturePlacementValid(furnitureType, tile) && tile.PendingFurnitureJob == null)
            {
                Job job = new Job(tile, furnitureType, (j) =>
                {
                    world.PlaceFurniture(furnitureType, j.Tile);
                    // TODO: I don't like having to manually and explicitly set flags that
                    //       prevent conflicts. It's too easy to forget to set/clear them!
                    tile.PendingFurnitureJob = null;
                });

                // TODO: I don't like having to manually and explicitly set flags that
                //       prevent conflicts. It's too easy to forget to set/clear them!
                tile.PendingFurnitureJob = job;

                job.RegisterJobCancelCallback((theJob) =>
                {
                    theJob.Tile.PendingFurnitureJob = null;
                });

                world.jobQueue.Enqueue(job);
            }
        }
        else
        {
            tile.Type = _buildModeTile;
        }
    }

    // TODO: For DEBUG use only
    public void DoPathfindingTestLevel()
    {
        WorldController.WorldData.SetupPathfindingExample();
    }
}
