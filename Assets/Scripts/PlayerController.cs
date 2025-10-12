using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Dependencies
    [SerializeField] WorldHandler world;
    #endregion
    #region Public Fields
    #endregion
    #region Private Variables
    private int[] worldSize;
    private float voxelSize;
    private bool initialised = false;
    #endregion

    #region Start
    void Start()
    {
    }
    #endregion
    #region Initialise
    private void CheckToInitialise()
    {
        if (world.worldCreated)
        {
            InitialiseSize();
            InitialisePosition();
            initialised = true;
        }
    }
    private void InitialiseSize()
    {
        voxelSize = world.voxelSize;
        Vector3 size = new Vector3(voxelSize - 0.1f, voxelSize - 0.1f, voxelSize - 0.1f);
        transform.localScale = size;
    }
    private void InitialisePosition()
    {
        worldSize = world.worldSize;
        float pixelSizeOffset = voxelSize / 2;
        float zPositionStart = pixelSizeOffset + worldSize[1] * 2f - worldSize[0] / 2 - 1;
        Vector3 closestBlockToCentre;
        closestBlockToCentre.x = 9999;
        closestBlockToCentre.y = 9999;
        closestBlockToCentre.z = 0;
        Vector3 playerPosition = Vector3.zero;
        for (int i = 0; i < world.blockPositions.Count; i++)
        {
            if (world.blockPositions[i].voxelPosition.z == zPositionStart)
            {
                if (((Mathf.Abs(world.blockPositions[i].voxelPosition.x) + Mathf.Abs(world.blockPositions[i].voxelPosition.y)) / 2) <= ((Mathf.Abs(closestBlockToCentre.x) + Mathf.Abs(closestBlockToCentre.y)) / 2))
                {
                    closestBlockToCentre = world.blockPositions[i].voxelPosition;
                    if (!world.blockPositions[i].voxelInPosition) playerPosition = world.blockPositions[i].voxelPosition;
                }
            }
        }
        if (playerPosition == Vector3.zero)
        {
            for (int i = 0; i < world.blockPositions.Count; i++)
            {
                if (world.blockPositions[i].voxelPosition.z == zPositionStart && !world.blockPositions[i].voxelInPosition)
                {
                    playerPosition = world.blockPositions[i].voxelPosition;
                }
            }
        }
        playerPosition.z += 0.1f;
        transform.position = playerPosition;
    }
    #endregion

    #region Update
    void Update()
    {
        if (!initialised) CheckToInitialise();
    }
    #endregion

    #region Movement
    public void HandleMovement()
    {
    }
    #endregion
}
