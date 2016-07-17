using UnityEngine;
using UnityEngine.UI;


// Every frame this script checks to see what tile the mouse is over and updates the TileInspector UI accordingly
public class TileInspector : MonoBehaviour
{
    private string defaultTileType = "Tile Type: Empty";
    private string defaultRoomIndex = "Room Index: 0000"; // TODO: Display room name rather than index
    private string defaultFurnitureType = "Furniture Type: Empty";

    public Text tileTypeText;
    public Text roomIndexText;
    public Text furnitureTypeText;
    

    void FixedUpdate()
    {
        // if mouse position is the same as last mouse position, do tooltip delay/transition

        // Update tile inspector UI
        tileTypeText.text = defaultTileType;
        roomIndexText.text = defaultRoomIndex;
        furnitureTypeText.text = defaultFurnitureType;
    }
}