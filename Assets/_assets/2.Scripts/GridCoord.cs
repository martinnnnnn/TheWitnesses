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

        bool _endOfLine = false;
        
        void Awake()
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
            _endOfLine = !_endOfLine;
            Debug.Log("end:" + _endOfLine);
            SetActivated();
            GetComponent<MeshRenderer>().material.color = Color.blue;
            SetEndOfLine();
        }

        void SetEndOfLine()
        {
            if (_endOfLine)
            {
                GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
        }

        public void SetActivated()
        {
            _activated = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public void Reset()
        {
            _owned = false;
            _activated = false;
            _endOfLine = false;
            GetComponent<MeshRenderer>().material.color = Color.white;
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

        public bool isEndOfLine()
        {
            return _endOfLine;
        }

    }
}


