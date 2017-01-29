using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace TheWitnesses
{
    public class LineHandler : NetworkBehaviour
    {

        GridCoord[] _coords;
        LineRenderer _line;

        void Awake()
        {
            _line = GetComponentInChildren<LineRenderer>();
            _line.SetPositions(new Vector3[2]);

            _coords = new GridCoord[2];

        }


        public void SetPositions(GridCoord coord1, GridCoord coord2)
        {
            _coords[0] = coord1;
            _coords[1] = coord2;

            _line.SetPosition(0, coord1.transform.position);
            _line.SetPosition(1, coord2.transform.position);
        }


        [ClientRpc]
        public void RpcSetPositions(GameObject firstCoord, GameObject sndCoord)
        {
            GridCoord coord1 = firstCoord.GetComponent<GridCoord>();
            GridCoord coord2 = sndCoord.GetComponent<GridCoord>();
            _coords[0] = coord1;
            _coords[1] = coord2;

            _line.SetPosition(0, coord1.transform.position);
            _line.SetPosition(1, coord2.transform.position);
        }


    }
}


