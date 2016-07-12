using UnityEngine;
using System.Collections;


static public class FurnitureActions
{
    static public void Door_UpdateAction(Furniture furn, float deltaTime)
    {
        //openPercentage = Mathf.Clamp01(openPercentage + (deltaTime / doorOpenTime) * DoorDirection);

        float openPercentage = furn.FurnitureParameter("openPercentage");
        float doorSpeed = 8;/* furn.FurnitureParameters["doorSpeed"]; */
        if (furn.FurnitureParameter("isOpening") >= 1)
        {
            openPercentage += doorSpeed * deltaTime;

            if (openPercentage >= 1)
            {
                furn.FurnitureParameter("isOpening", 0);
            }
        }
        else
        {
            openPercentage -= doorSpeed * deltaTime;
        }

        furn.FurnitureParameter("openPercentage", Mathf.Clamp01(openPercentage));

        if (furn.cbOnChanged != null)
        {
            furn.cbOnChanged(furn);
        }
    }

    static public Enterability Door_RequestEntrance(Furniture furn)
    {
        furn.FurnitureParameter("isOpening", 1);

        if (furn.FurnitureParameter("openPercentage") >= 1)
        {
            return Enterability.Yes;
        }

        return Enterability.Soon;
    }
}