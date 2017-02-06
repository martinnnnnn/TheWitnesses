using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


namespace TheWitnesses
{
    public class Character : NetworkBehaviour
    {

        public bool smooth;
        public bool lockCursor;
        public float upDownRange = 60;

        GridController Grid;
        public GridManager gridManager;

        private Transform _camera;

        private bool _unLockCursor;
        private CharacterController _character;
        private float rotUpDown = 0;

        public LayerMask layerMask;

        //public GridCoord currentPoint;

        public override void OnStartLocalPlayer()
        {
            //GetComponent<MeshRenderer>().material.color = Color.red;
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        }


        void Start()
        {
            //gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            if (!isLocalPlayer)
            {
                //Destroy(this);
                return;
            }
            //gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            Debug.Log("name:" + gridManager.currentGrid.name);
            //Grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridController>();
            //Grid = GameObject.Find("Grid(Clone)").GetComponent<GridController>();

            _camera = Camera.main.transform;
            SetCamera();
            _character = GetComponent<CharacterController>();

            _unLockCursor = true;
        }

        void SetCamera()
        {
            Transform camStart = transform.Find("CameraStartingPosition").transform;
            _camera.position = camStart.position;
            _camera.rotation = camStart.rotation;
            _camera.SetParent(transform);
        }


        void Update()
        {
            if (!isLocalPlayer)
                return;

            // Mouse Rotation
            float rotLeftRight = Input.GetAxis("Mouse X");
            transform.Rotate(0, rotLeftRight, 0);

            rotUpDown -= Input.GetAxis("Mouse Y");
            rotUpDown = Mathf.Clamp(rotUpDown, -upDownRange, upDownRange);
            _camera.transform.localRotation = Quaternion.Euler(rotUpDown, 0, 0);



            // Movement
            float movSide = Input.GetAxis("Horizontal") * 5f;
            float movForward = Input.GetAxis("Vertical") * 5f;
            Vector3 movement = new Vector3(movSide, 0, movForward);
            movement = transform.rotation * movement;
            _character.SimpleMove(movement);


            CursorLockUpdate();

            if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Ping(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                Ping(false);
            }

            if (Input.GetButton("Fire1"))
            {
                CmdResetGrid();
            }

        }

        void Ping(bool isPinging)
        {
            if (isPinging)
            {
                RaycastHit info;
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out info, 100, layerMask))
                {
                    GridCoord coord = info.collider.GetComponent<GridCoord>();
                    if (coord)
                    {
                        CmdPing(coord.gameObject);
                    }
                }
            }
            else
            {
                CmdDestroyFXPing();
            }
        }

        [Command]
        void CmdDestroyFXPing()
        {

            Destroy(FXPing);
            FXPing = null;
        }

        public GameObject FXPingPrefab;
        GameObject FXPing;

        [Command]
        void CmdPing(GameObject obj)
        {
            FXPing = Instantiate(FXPingPrefab, obj.transform.position, obj.transform.rotation);
            NetworkServer.Spawn(FXPing);
        }

        //[Command]
        void Fire()
        {
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
            if (Physics.Raycast(ray, out info, 100, layerMask))
            {
                GridCoord coord = info.collider.GetComponent<GridCoord>();
                if (coord)
                {
                    CmdSetCoord(coord.gameObject);
                    return;
                }
                GridController controller = info.collider.GetComponent<GridController>();
                if (controller)
                {
                    Debug.Log("gello");
                    
                    Grid = controller;
                    return;
                }
            }

        }

        [SyncVar]
        private GameObject objectID;
        private NetworkIdentity objNetId;

        [Command]
        void CmdSetCoord(GameObject coord)
        {
            if (!gridManager.currentGrid)
            {
                return;
                //Grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridController>();
            }

            objNetId = gridManager.currentGrid.GetComponent<NetworkIdentity>();
            objNetId.AssignClientAuthority(connectionToClient);
            gridManager.currentGrid.RpcSetCoord(coord,gameObject);
            objNetId.RemoveClientAuthority(connectionToClient);

        }

        [Command]
        void CmdResetGrid()
        {
            if (!gridManager.currentGrid)
            {
                return;
                //Grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridController>();
            }

            objNetId = gridManager.currentGrid.GetComponent<NetworkIdentity>();
            objNetId.AssignClientAuthority(connectionToClient);
            gridManager.currentGrid.RpcReset();
            objNetId.RemoveClientAuthority(connectionToClient);

            List<LineHandler> lines = gridManager.currentGrid.GetLines();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i])
                {
                    Destroy(lines[i].gameObject);
                }
            }
            lines.Clear();

        }

        public Material endGridMaterial;
        [Command]  
        public void CmdEndGrid (GameObject endPointPosition, GameObject cable)
        {
            // spawn special FX
            // deactivate grid
            // activate lien
            gridManager.currentGrid.RpcReset();
            gridManager.currentGrid = null;
            cable.GetComponent<Renderer>().material = endGridMaterial;
        }


        private void CursorLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _unLockCursor = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _unLockCursor = true;
            }

            if (_unLockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!_unLockCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public GameObject LinePrefab;

        [Command]
        public void CmdCreateNewLine(GameObject firstCoord, GameObject sndCoord)
        {
            if (!gridManager.currentGrid)
            {
                return;
            }

            if (firstCoord != sndCoord)
            {
                GameObject newLine = Instantiate(LinePrefab, gridManager.currentGrid.transform);
                LineHandler newLineHandler = newLine.GetComponent<LineHandler>();
                gridManager.currentGrid.AddLine(newLineHandler);
                NetworkServer.Spawn(newLine);

                objNetId = newLine.GetComponent<NetworkIdentity>();
                objNetId.AssignClientAuthority(connectionToClient);
                newLineHandler.RpcSetPositions(firstCoord, sndCoord);
                //newLineHandler.SetPositions(firstCoord.GetComponent<GridCoord>(), sndCoord.GetComponent<GridCoord>());
                objNetId.RemoveClientAuthority(connectionToClient);



            }
        }


        public GameObject FXSpawnNewPoint;
        [Command]
        public void CmdSpawnNewPointFX(GameObject obj)
        {
            GameObject newFX = Instantiate(FXSpawnNewPoint, obj.transform.position,obj.transform.rotation);
            NetworkServer.Spawn(newFX);
            Destroy(newFX, 0.50f);
        }

        [Command]
        public void CmdCallCheckLines(GameObject coord)
        {
            objNetId = gridManager.currentGrid.GetComponent<NetworkIdentity>();
            objNetId.AssignClientAuthority(connectionToClient);
            gridManager.currentGrid.RpcCheckLines(coord);
            objNetId.RemoveClientAuthority(connectionToClient);
        }
    }
}


