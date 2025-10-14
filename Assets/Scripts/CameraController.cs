using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Unity.Cinemachine;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    #region Dependencies
    [SerializeField] GameObject player;
    new CinemachineCamera camera;
    #endregion
    #region Public Fields
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
    public void InitialiseCameraPosition(int worldDepth, Vector3 vectorToGrid)
    {
        transform.position = vectorToGrid * -1 * worldDepth * 10;
        camera.Lens.FieldOfView = worldDepth + worldDepth / 4;
    }
    #endregion

    #region Update
    void Update()
    {
    }
    #endregion

    #region Switching Faces
    public void SwitchFace(int worldDepth, Vector3 vectorToGrid)
    {
        Vector3 positionToSwitchTo = Vector3.zero;
        Vector3 rotationToSwitchTo = Vector3.zero;
        positionToSwitchTo = vectorToGrid * -1 * worldDepth * 10;
        transform.position = positionToSwitchTo; //                <- smooth out position switching 
        camera.Lens.FieldOfView = worldDepth + worldDepth / 4;
        if (vectorToGrid.z > 0) rotationToSwitchTo = Vector3.zero;
        else if (vectorToGrid.z < 0) rotationToSwitchTo = Vector3.down * 180;
        else if (vectorToGrid.x < 0) rotationToSwitchTo = Vector3.down * 90;
        else if (vectorToGrid.x > 0) rotationToSwitchTo = Vector3.up * 90;
        else if (vectorToGrid.y < 0) rotationToSwitchTo = Vector3.right * 90;
        else if (vectorToGrid.y > 0) rotationToSwitchTo = Vector3.left * 90;
        transform.localEulerAngles = rotationToSwitchTo; //            <- smooth out rotation 
    }
    #endregion
}