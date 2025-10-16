using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Unity.Cinemachine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    #region Dependencies
    [SerializeField] PlayerController player;
    new CinemachineCamera camera;
    #endregion
    #region Public Fields
    public bool rotationDone = false;
    public bool faceSwitched = false;
    #endregion
    #region Private Variables
    private Vector3 rotationToSwitchTo = Vector3.zero;
    #endregion

    #region Start
    void Start()
    {
        camera = GetComponentInChildren<CinemachineCamera>();
    }
    #endregion
    #region Initialise
    public void InitialiseCameraPosition(Vector3 depthVector, int worldDepth)
    {
        transform.position = depthVector * -1 * worldDepth * 10;
        camera.Lens.FieldOfView = worldDepth + worldDepth / 4;
    }
    #endregion

    #region Update
    void Update()
    {
    }
    #endregion

    #region Switching Faces
    public void RotateCameraForSwitch(Vector3 faceChange)
    {
        Vector3 rotationToSwitchTo;
        rotationToSwitchTo.x = transform.rotation.x;
        rotationToSwitchTo.y = transform.rotation.y;
        rotationToSwitchTo.z = transform.rotation.z;
        if (player.currentFace == 0 && faceChange == Vector3.left || player.currentFace == 2 && faceChange == Vector3.forward || player.currentFace == 1 && faceChange == Vector3.right || player.currentFace == 3 && faceChange == Vector3.back)
        {
            rotationToSwitchTo.y += 90;
        }
        else if (player.currentFace == 0 && faceChange == Vector3.right || player.currentFace == 3 && faceChange == Vector3.forward || player.currentFace == 1 && faceChange == Vector3.left || player.currentFace == 2 && faceChange == Vector3.back)
        {
            rotationToSwitchTo.y -= 90;
        }
        else if (player.currentFace == 0 && faceChange == Vector3.up || player.currentFace == 4 && faceChange == Vector3.forward || player.currentFace == 1 && faceChange == Vector3.down || player.currentFace == 5 && faceChange == Vector3.back)
        {
            rotationToSwitchTo.x += 90;
        }
        else if (player.currentFace == 0 && faceChange == Vector3.down || player.currentFace == 5 && faceChange == Vector3.forward || player.currentFace == 1 && faceChange == Vector3.up || player.currentFace == 4 && faceChange == Vector3.back)
        {
            rotationToSwitchTo.x -= 90;
        }
        else if (player.currentFace == 2 && faceChange == Vector3.up || player.currentFace == 4 && faceChange == Vector3.right || player.currentFace == 3 && faceChange == Vector3.down || player.currentFace == 5 && faceChange == Vector3.left)
        {
            rotationToSwitchTo.x += 90;
        }
        else if (player.currentFace == 3 && faceChange == Vector3.up || player.currentFace == 4 && faceChange == Vector3.left || player.currentFace == 2 && faceChange == Vector3.down || player.currentFace == 5 && faceChange == Vector3.right)
        {
            rotationToSwitchTo.x -= 90;
        }
        if (rotationToSwitchTo.x < 0) rotationToSwitchTo.x += 360;
        if (rotationToSwitchTo.y < 0) rotationToSwitchTo.y += 360;
        if (rotationToSwitchTo.z < 0) rotationToSwitchTo.z += 360;
        if (rotationToSwitchTo.x >= 360) rotationToSwitchTo.x -= 360;
        if (rotationToSwitchTo.y >= 360) rotationToSwitchTo.y -= 360;
        if (rotationToSwitchTo.z >= 360) rotationToSwitchTo.z -= 360;
        Debug.Log(rotationToSwitchTo);
        transform.localEulerAngles = rotationToSwitchTo; //          <- smooth out rotation switching 
        rotationDone = true;
    }
    public void SwitchFace(Vector3 worldSize)
    {
        int worldDepth = 0;
        if (player.movementInstructions[player.currentFace,4].x != 0) worldDepth = (int)worldSize.x;
        else if (player.movementInstructions[player.currentFace,4].y != 0) worldDepth = (int)worldSize.y;
        else if (player.movementInstructions[player.currentFace,4].z != 0) worldDepth = (int)worldSize.z;
        Vector3 positionToSwitchTo;
        positionToSwitchTo = player.movementInstructions[player.currentFace,4] * -1 * worldDepth * 10;
        transform.position = positionToSwitchTo; //                <- smooth out position switching 
        camera.Lens.FieldOfView = worldDepth + worldDepth / 4;
        faceSwitched = true;
    }
    #endregion
}