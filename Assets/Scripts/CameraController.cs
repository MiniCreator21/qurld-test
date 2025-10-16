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
using System.ComponentModel;

public class CameraController : MonoBehaviour
{
    #region Dependencies
    [SerializeField] WorldHandler world;
    [SerializeField] PlayerController player;
    new CinemachineCamera camera;
    #endregion
    #region Public Fields
    public Vector3 currentUpVector = Vector3.up;
    public bool faceSwitched = false;
    #endregion
    #region Private Variables
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
    public void SwitchFace(Vector2 faceChange)
    {
        int worldDepth = 0;
        if (player.movementInstructions[player.currentFace, 4].x != 0) worldDepth = (int)world.worldSize.x;
        else if (player.movementInstructions[player.currentFace, 4].y != 0) worldDepth = (int)world.worldSize.y;
        else if (player.movementInstructions[player.currentFace, 4].z != 0) worldDepth = (int)world.worldSize.z;
        Vector3 oldCameraPosition = transform.position;
        Vector3 newCameraPosition;
        newCameraPosition = player.movementInstructions[player.currentFace,4] * -1 * worldDepth * 10;
        transform.position = newCameraPosition; //                <- smooth out position switching 
        camera.Lens.FieldOfView = worldDepth + worldDepth / 4;
        Vector3 newVectorToCube = (world.transform.position - newCameraPosition).normalized;
        if (faceChange == Vector2.left)
        {
            //currentUpVector = currentUpVector;
            transform.rotation = Quaternion.LookRotation(newVectorToCube, currentUpVector);
            //Debug.Log("Horizontal Left Rotate");
        }
        else if (faceChange == Vector2.right)
        {
            //currentUpVector = currentUpVector;
            transform.rotation = Quaternion.LookRotation(newVectorToCube, currentUpVector);
            //Debug.Log("Horizontal Right Rotate");
        }
        else if (faceChange == Vector2.up)
        {
            currentUpVector = (world.transform.position - oldCameraPosition).normalized;
            transform.rotation = Quaternion.LookRotation(newVectorToCube, currentUpVector);
            //Debug.Log("Front Vertical Up Rotate");
        }
        else if (faceChange == Vector2.down)
        {
            currentUpVector = (oldCameraPosition - world.transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(newVectorToCube, currentUpVector);
            //Debug.Log("Front Vertical Down Rotate");
        }
        faceSwitched = true;
    }
    #endregion
}