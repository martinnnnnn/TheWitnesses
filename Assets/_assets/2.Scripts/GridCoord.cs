using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace TheWitnesses
{

    public struct IntVector2
    {
        public int x;
        public int y;
    }

    public class GridCoord : MonoBehaviour
    {


        public int x;
        public int y;
        IntVector2 gridPosition;

        bool _activated = false;
        bool _owned = false;
        
        void Start()
        {
            gridPosition.x = x;
            gridPosition.y = y;
        }

        public IntVector2 GetPosition()
        {
            return gridPosition;
        }


        public void SetOwned()
        {
            _owned = true;
            SetActivated();
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }

        public void SetActivated()
        {
            Debug.Log("color");
            _activated = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public void Reset()
        {
            _owned = false;
            _activated = false;

        }

        public bool IsAvailable()
        {
            return (!_activated && !_owned);
        }

        public bool isOwned()
        {
            return _owned;
        }

        public bool isActivated()
        {
            return _activated;
        }

    }
}


