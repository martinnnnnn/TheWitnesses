using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace TheWitnesses
{

    
    public class GridController : MonoBehaviour
    {

        GridCoord[] _coords;


        void Start()
        {
            _coords = new GridCoord[16];
        }


        void SetCoord(GridCoord coord)
        {
            if (Owned(coord))
            {
                if (coord.IsAvailable())
                {
                    //coord.SetAvailability(true);
                    CheckLines(coord.GetPosition());
                }
            }
        }


        bool Owned(GridCoord coord)
        {
            for (int i = 0; i < _coords.Length; i++)
            {
                if (coord == _coords[i])
                {
                    return true;
                }
            }
            return false;
        }

        void CheckLines(Vector2 coords)
        {

        }
    }
}


