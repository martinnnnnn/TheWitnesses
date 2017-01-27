using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace TheWitnesses
{
    public class Character : NetworkBehaviour
    {

        public bool smooth;
        public bool lockCursor;
        public float upDownRange = 60;


        private Camera _camera;

        private bool _unLockCursor;
        private CharacterController _character;
        private float rotUpDown = 0;

        public LayerMask layerMask;


        //public override void OnStartLocalPlayer()
        //{
        //    GetComponent<MeshRenderer>().material.color = Color.red;
        //}


        void Start()
        {
            if (!isLocalPlayer)
            {
                GetComponentInChildren<Camera>().enabled = false;
                return;
            }

            _camera = GetComponentInChildren<Camera>();
            _camera.enabled = true;

            _character = GetComponent<CharacterController>();

            _unLockCursor = true;
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

        }

        //[Command]
        void Fire()
        {
            RaycastHit info;
            Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
            if (Physics.Raycast(ray, out info, 100, layerMask))
            {
                GridCoord coord = info.collider.GetComponent<GridCoord>();
                if (coord)
                {
                    Debug.Log("coord:" + coord.GetPosition().x + "," + coord.GetPosition().y);
                }
            }

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


    }
}


