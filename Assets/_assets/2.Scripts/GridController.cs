using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


namespace TheWitnesses
{

    
    public class GridController : NetworkBehaviour
    {

        public GridCoord[] _coordsArray;

        GridCoord[][] _coordsMatrix;

        public GameObject LinePrefab;
        List<LineHandler> _lines;

        public GridCoord startPoint;
        public GridCoord endPoint;

        public GridCoord currentPoint;

        public GameObject cable;

        void Start()
        {
            currentPoint = startPoint;
            _lines = new List<LineHandler>();

            for (int i = 0; i < _coordsArray.Length; i++)
            {
                _coordsArray[i] = null;
            }

            int j = 0;
            foreach (Transform child in transform)
            {
                if (child.tag == ("GridCoord"))
                {
                    _coordsArray[j] = child.GetComponent<GridCoord>();
                    j++;
                }
            }
            //GameObject[] objs = GameObject.FindGameObjectsWithTag("GridCoord");
            //Debug.Log("size:" + objs.Length);
            //for (int i = 0; i < objs.Length; i++)
            //{
            //    _coordsArray[i] = objs[i].GetComponent<GridCoord>();
            //    //Debug.Log("i: " + i + "(" + _coordsArray[i].GetPosition().x + "," + _coordsArray[i].GetPosition().y + ")");
            //}

            InitGrid();
            
        }

        GridCoord _newCoord;
        void Update()
        {
            //if (!isServer)
            //    return;

            //if (_newCoord != null)
            //{
            //    List<GridCoord> newline = CheckLines(_newCoord);
            //    ActivateCoord(_newCoord, newline);
            //    ActivateLine(_newCoord, newline);
            //    _newCoord = null;
            //}
        }

        Character localClient;

        [ClientRpc]
        public void RpcSetCoord(GameObject gridObj, GameObject currentClient)
        {
            Debug.Log("rpcSetCoord");
            localClient = currentClient.GetComponent<Character>();
            GridCoord coord = gridObj.GetComponent<GridCoord>();
            //for (int i = 0; i < _coordsMatrix.Length; i++)
            //{
            //    for (int j = 0; j < _coordsMatrix[i].Length; j++)
            //    {
            //        //Debug.Log(i + "," + j);
            //        Debug.Log("rpc: (" + _coordsMatrix[i][j].GetPosition().x + "," + _coordsMatrix[i][j].GetPosition().y + ")");
            //    }
            //}

            if (Good(coord))
            {
                //localClient.CmdCallCheckLines(coord.gameObject);
                List<GridCoord> newline = CheckLines(coord);
                ActivateCoord(coord, newline);
            }
        }


        [ClientRpc]
        public void RpcReset()
        {
            for(int i = 0; i < _coordsMatrix.Length; i++)
            {
                for (int j = 0; j < _coordsMatrix[i].Length; j++)
                {
                    _coordsMatrix[i][j].Reset();
                }
            }
            Debug.Log("RESET");
            currentPoint = startPoint;
            startPointDone = false;


        }


        public void AddLine(LineHandler handler)
        {
            _lines.Add(handler);
        }

        public List<LineHandler> GetLines()
        {
            return _lines;
        }

        // active les nouvelles lignes
        void ActivateCoord(GridCoord firstCoord, List<GridCoord> line)
        {
            Debug.Log("size:" + line.Count);
            if (line.Count == 0)
            {
                return;
            }
            firstCoord.SetOwned();
            if (localClient)
            {
                localClient.CmdSpawnNewPointFX(firstCoord.gameObject);
            }

            foreach (GridCoord sndCoord in line)
            {
                

                if (sndCoord!= firstCoord)
                {
                    if (localClient)
                    {
                        sndCoord.SetOwned();
                        //Debug.Log("line drawn");
                        localClient.CmdCreateNewLine(firstCoord.gameObject, sndCoord.gameObject);
                        if (sndCoord == endPoint || firstCoord == endPoint)
                        {
                            
                            //Debug.Log("hello maggle");
                            localClient.CmdEndGrid(endPoint.gameObject,cable);
                        }
                    }
                }

                // activation des points entre les 2 points principaux
                if (firstCoord.GetPosition().x == sndCoord.GetPosition().x)
                {
                    int x = firstCoord.GetPosition().x;
                    int lowY = (firstCoord.GetPosition().y < sndCoord.GetPosition().y) ? firstCoord.GetPosition().y : sndCoord.GetPosition().y;
                    int highY = (firstCoord.GetPosition().y < sndCoord.GetPosition().y) ? sndCoord.GetPosition().y : firstCoord.GetPosition().y;

                    for (int i = lowY + 1; i < highY; i++)
                    {
                        _coordsMatrix[x][i].SetActivated();
                    }
                }
                else if (firstCoord.GetPosition().y == sndCoord.GetPosition().y)
                {
                    int y = firstCoord.GetPosition().y;
                    int lowX = (firstCoord.GetPosition().x < sndCoord.GetPosition().x) ? firstCoord.GetPosition().x : sndCoord.GetPosition().x;
                    int highX = (firstCoord.GetPosition().x < sndCoord.GetPosition().x) ? sndCoord.GetPosition().x : firstCoord.GetPosition().x;

                    for (int i = lowX + 1; i < highX; i++)
                    {
                        _coordsMatrix[i][y].SetActivated();
                    }
                }
                
            }
        }

        // active les nouvelles lignes
        void ActivateLine(GridCoord firstCoord, List<GridCoord> line)
        {
            foreach (GridCoord sndCoord in line)
            {
                //shiny line between 2 points
            }
        }




        bool Good(GridCoord coord)
        {
            if (coord)
            {
                //Debug.Log("coord");
                //Debug.Log("coord: (" + coord.GetPosition().x + "," + coord.GetPosition().y + ")");
                //Debug.Log("coord: (" + _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y].GetPosition().x + "," + _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y].GetPosition().y + ")");
                //Debug.Log(coord.name + ", " + _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y].name);
                if (coord == _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y])
                {
                    //Debug.Log("good position");

                    if (coord.IsAvailable())
                    {
                        //Debug.Log("av");

                        return true;
                    }
                }
            }
            return false;
        }


        bool startPointDone = false;
        // renvoie la liste de points avec lesquels le point coord va former des nouvelles lignes
        List<GridCoord> CheckLines(GridCoord firstCoord)
        {
            List<GridCoord> newPoints = new List<GridCoord>();

            if (!startPointDone)
            {
                Debug.Log("start");
                if (firstCoord == startPoint)
                {
                    newPoints.Add(firstCoord);
                    newPoints.Add(startPoint);
                    startPointDone = true;
                    currentPoint = firstCoord;
                }
                return newPoints;
            }
            else
            {

                if (currentPoint.GetPosition().x == firstCoord.GetPosition().x)
                {
                    

                    int x = currentPoint.GetPosition().x;
                    int lowY = (firstCoord.GetPosition().y < currentPoint.GetPosition().y) ? firstCoord.GetPosition().y : currentPoint.GetPosition().y;
                    int highY = (firstCoord.GetPosition().y < currentPoint.GetPosition().y) ? currentPoint.GetPosition().y : firstCoord.GetPosition().y;

                    bool canAdd = true;
                    for (int i = lowY + 1; i < highY; i++)
                    {
                        if (_coordsMatrix[x][i].isActivated())
                        {
                            canAdd = false;
                        }
                    }
                    if (canAdd)
                    {
                        newPoints.Add(firstCoord);
                        newPoints.Add(currentPoint);
                        currentPoint = firstCoord;
                    }
                }
                else if (currentPoint.GetPosition().y == firstCoord.GetPosition().y)
                {
                    int y = currentPoint.GetPosition().y;
                    int lowX = (firstCoord.GetPosition().x < currentPoint.GetPosition().x) ? firstCoord.GetPosition().x : currentPoint.GetPosition().x;
                    int highX = (firstCoord.GetPosition().x < currentPoint.GetPosition().x) ? currentPoint.GetPosition().x : firstCoord.GetPosition().x;

                    bool canAdd = true;
                    for (int j = lowX + 1; j < highX; j++)
                    {
                        if (_coordsMatrix[j][y].isActivated())
                        {
                            canAdd = false;
                        }
                    }
                    if (canAdd)
                    {
                        newPoints.Add(firstCoord);
                        newPoints.Add(currentPoint);
                        currentPoint = firstCoord;
                    }
                }

            }
            //Debug.Log("first : (" + firstCoord.GetPosition().x + "," + firstCoord.GetPosition().y + ")");
            //Debug.Log("current : (" + currentPoint.GetPosition().x + "," + currentPoint.GetPosition().y + ")");

            //foreach (GridCoord coord in newPoints)
            //{
            //    Debug.Log("(" + coord.GetPosition().x + "," + coord.GetPosition().y + ")");
            //}
            return newPoints;


            //List<GridCoord> newPoints = new List<GridCoord>();
            //newPoints.Add(firstCoord);

            //int x = firstCoord.GetPosition().x;
            //for (int j = 0; j < _coordsMatrix[x].Length; j++)
            //{
            //    GridCoord sndCoord = _coordsMatrix[x][j];
            //    if (sndCoord.isOwned())
            //    {

            //        // trouver les points entre first et second
            //        // si un est available -> nop

            //        int lowY = (firstCoord.GetPosition().y < sndCoord.GetPosition().y) ? firstCoord.GetPosition().y : sndCoord.GetPosition().y;
            //        int highY = (firstCoord.GetPosition().y < sndCoord.GetPosition().y) ? sndCoord.GetPosition().y : firstCoord.GetPosition().y;

            //        bool canAdd = true;
            //        for (int i = lowY + 1; i < highY; i++)
            //        {
            //            if (_coordsMatrix[x][i].isActivated())
            //            {
            //                canAdd = false;
            //            }
            //        }
            //        if (canAdd)
            //        {
            //            newPoints.Add(sndCoord);
            //        }
            //    }
            //}

            //int y = firstCoord.GetPosition().y;
            //for (int i = 0; i < _coordsMatrix.Length; i++)
            //{
            //    GridCoord sndCoord = _coordsMatrix[i][y];
            //    if (sndCoord.isOwned())
            //    {
            //        int lowX = (firstCoord.GetPosition().x < sndCoord.GetPosition().x) ? firstCoord.GetPosition().x : sndCoord.GetPosition().x;
            //        int highX = (firstCoord.GetPosition().x < sndCoord.GetPosition().x) ? sndCoord.GetPosition().x : firstCoord.GetPosition().x;

            //        bool canAdd = true;
            //        for (int j = lowX + 1; j < highX; j++)
            //        {
            //            if (_coordsMatrix[j][y].isActivated())
            //            {
            //                canAdd = false;
            //            }
            //        }
            //        if (canAdd)
            //        {
            //            newPoints.Add(sndCoord);
            //        }



            //        //newPoints.Add(_coordsMatrix[i][y]);
            //    }
            //}

            //return newPoints;
        }

        void InitGrid()
        {
            _coordsMatrix = new GridCoord[4][];
            for (int i = 0; i < _coordsMatrix.Length; i++)
            {
                _coordsMatrix[i] = new GridCoord[4];
            }

            foreach (GridCoord coord in _coordsArray)
            {
                IntVector2 pos = coord.GetPosition();
                _coordsMatrix[pos.x][pos.y] = coord;
            }

            //for (int i = 0; i < _coordsMatrix.Length; i++)
            //{
            //    for (int j = 0; j < _coordsMatrix[i].Length; j++)
            //    {
            //        //Debug.Log(i + "," + j);
            //        Debug.Log("init: (" + _coordsMatrix[i][j].GetPosition().x + "," + _coordsMatrix[i][j].GetPosition().y + ")");
            //    }
            //}
        }

        [ClientRpc]
        public void RpcCheckLines(GameObject coord)
        {
            GridCoord firstCoord = coord.GetComponent<GridCoord>();
            List<GridCoord> newPoints = new List<GridCoord>();

            if (!startPointDone)
            {
                if (firstCoord == startPoint)
                {
                    newPoints.Add(firstCoord);
                    newPoints.Add(startPoint);
                    startPointDone = true;
                    currentPoint = firstCoord;
                }
            }
            else
            {
                if (currentPoint.GetPosition().x == firstCoord.GetPosition().x)
                {
                    int x = currentPoint.GetPosition().x;
                    int lowY = (firstCoord.GetPosition().y < currentPoint.GetPosition().y) ? firstCoord.GetPosition().y : currentPoint.GetPosition().y;
                    int highY = (firstCoord.GetPosition().y < currentPoint.GetPosition().y) ? currentPoint.GetPosition().y : firstCoord.GetPosition().y;

                    bool canAdd = true;
                    for (int i = lowY + 1; i < highY; i++)
                    {
                        if (_coordsMatrix[x][i].isActivated())
                        {
                            canAdd = false;
                        }
                    }
                    if (canAdd)
                    {
                        newPoints.Add(firstCoord);
                        newPoints.Add(currentPoint);
                        currentPoint = firstCoord;
                    }
                }
                else if (currentPoint.GetPosition().y == firstCoord.GetPosition().y)
                {
                    int y = currentPoint.GetPosition().y;
                    int lowX = (firstCoord.GetPosition().x < currentPoint.GetPosition().x) ? firstCoord.GetPosition().x : currentPoint.GetPosition().x;
                    int highX = (firstCoord.GetPosition().x < currentPoint.GetPosition().x) ? currentPoint.GetPosition().x : firstCoord.GetPosition().x;

                    bool canAdd = true;
                    for (int j = lowX + 1; j < highX; j++)
                    {
                        if (_coordsMatrix[j][y].isActivated())
                        {
                            canAdd = false;
                        }
                    }
                    if (canAdd)
                    {
                        newPoints.Add(firstCoord);
                        newPoints.Add(currentPoint);
                        currentPoint = firstCoord;
                    }
                }

            }
            //foreach (GridCoord c in newPoints)
            //{
            //    Debug.Log("(" + c.GetPosition().x + "," + c.GetPosition().y + ")");
            //}

            ActivateLine(firstCoord, newPoints);

        }
    }


}


