using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


namespace TheWitnesses
{

    
    public class GridController : NetworkBehaviour
    {

        float CoordGap;

        public GridCoord[] _coordsArray;
        GridCoord[][] _coordsMatrix;


        void Start()
        {
            
            InitGrid();

        }


        public void SetCoord(GridCoord coord)
        {
            Debug.Log("setcoord");
            if (Good(coord))
            {
                Debug.Log("good");
                List<GridCoord> newline = CheckLines(coord);
                ActivateCoord(coord,newline);
                ActivateLine(coord, newline);
            }
        }

        // active les nouvelles lignes
        void ActivateCoord(GridCoord firstCoord, List<GridCoord> line)
        {
            foreach (GridCoord sndCoord in line)
            {
                sndCoord.SetOwned();
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
                Debug.Log("notnull");
                Debug.Log("coord:" + coord.GetPosition().x + "," + coord.GetPosition().x + " / matrix:" + _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y].GetPosition().x + "," + _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y].GetPosition().y);
                if (coord == _coordsMatrix[coord.GetPosition().x][coord.GetPosition().y])
                {
                    Debug.Log("foundinmatrix");
                    if (coord.IsAvailable())
                    {
                        Debug.Log("available");
                        return true;
                    }
                }
            }
            return false;
        }
        
        // renvoie la liste de points avec lesquels le point coord va former des nouvelles lignes
        List<GridCoord> CheckLines(GridCoord coord)
        {
            List<GridCoord> newPoints = new List<GridCoord>();
            newPoints.Add(coord);

            int x = coord.GetPosition().x;
            for (int j = 0; j < _coordsMatrix[x].Length; j++)
            {
                if (!_coordsMatrix[x][j].IsAvailable())
                {
                    newPoints.Add(_coordsMatrix[x][j]);
                }
            }

            int y = coord.GetPosition().y;
            for (int i = 0; i < _coordsMatrix.Length; i++)
            {
                if (!_coordsMatrix[i][y].IsAvailable())
                {
                    newPoints.Add(_coordsMatrix[i][y]);
                }
            }

            return newPoints;
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
            
            //for (int i = 0; i <_coordsMatrix.Length; i++)
            //{
            //    for (int j = 0; j < _coordsMatrix[i].Length; j++)
            //    {
            //        Debug.Log(_coordsMatrix[i][j].GetPosition().x + "," + _coordsMatrix[i][j].GetPosition().y);
            //    }
            //}
        }
    }
}


