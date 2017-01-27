using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace TheWitnesses
{
    public class GridCoord : NetworkBehaviour
    {



        public Vector2 gridPosition;

        bool _available = true;
        //bool 


        public Vector2 GetPosition()
        {
            return gridPosition;
        }

        public bool IsAvailable()
        {
            return _available;
        }


    }
}


