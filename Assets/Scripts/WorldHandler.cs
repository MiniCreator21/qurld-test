using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework.Internal;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{
    #region Dependencies
    [SerializeField] private GameObject gridVoxel;
    [SerializeField] private GameObject blockVoxel;
    #endregion
    #region Public Fields
    #endregion
    #region Private Variables
    private int[] worldSize = { 8, 8, 8 };
    private List<int>[] worldSeed;
    [SerializeField] private float voxelSize = 1f;
    private List<GameObject> GridVoxels;
    #endregion
    #region Voxel Struct
    [System.Serializable]
    public struct VoxelPosition
    {
        public bool voxelInPosition;
        public Vector3 voxelPosition;
    }
    private List<VoxelPosition> gridPositions;
    private List<VoxelPosition> blockPositions;
    #endregion

    #region Start
    void Start()
    {
        CreateSeed(worldSize[0], worldSize[1], worldSize[2]);
        CreateWorld();
    }
    #endregion

    #region Update
    void Update()
    {

    }
    #endregion

    #region Seed Creation
    private void CreateSeed(int xSize, int ySize, int zSize)
    {
        worldSeed = new List<int>[6];
        for (int i = 0; i < 2; i++) //iterates through front and back sides of cube 
        {
            if (i == 0)
            {
                List<int> newFace = new List<int>
                {
                    1,0,0,0,1,0,0,0,
                    1,0,1,0,1,0,1,0,
                    0,0,0,0,0,0,0,0,
                    1,1,1,0,1,0,1,1,
                    1,0,0,0,1,0,0,0,
                    1,0,1,1,1,1,0,1,
                    1,0,0,0,0,0,0,1,
                    1,1,1,1,0,1,1,1
                };
                worldSeed[i] = newFace;
            }
            if (i == 1)
            {
                List<int> newFace = new List<int>
                {
                    0,1,0,0,0,0,0,0,
                    0,0,0,0,0,0,1,0,
                    0,0,0,0,0,0,0,0,
                    0,0,0,0,0,1,0,0,
                    0,0,0,0,0,0,0,1,
                    0,0,0,0,1,0,0,0,
                    0,0,1,0,1,0,0,0,
                    1,0,1,0,1,0,0,0
                };
                worldSeed[i] = newFace;
            }
        }
        for (int i = 0; i < 2; i++) //iterates through left and right sides of cube 
        {
            if (i == 0)
            {
                List<int> newFace = new List<int>
                {
                    1,1,1,1,1,1,1,1,
                    1,0,0,0,0,0,0,0,
                    1,0,0,0,0,0,1,0,
                    0,1,0,1,1,0,0,0,
                    0,1,0,0,1,0,0,1,
                    0,1,1,0,0,0,0,0,
                    0,0,1,0,1,1,0,0,
                    0,0,1,1,1,1,1,1
                };
                worldSeed[i + 2] = newFace;
            }
            if (i == 1)
            {
                List<int> newFace = new List<int>
                {
                    1,0,0,0,1,1,0,0,
                    1,1,1,0,0,0,0,1,
                    1,0,0,0,0,0,1,1,
                    1,0,0,0,0,0,0,1,
                    0,0,0,0,0,0,0,1,
                    1,1,1,1,1,1,0,1,
                    1,1,0,0,0,0,0,1,
                    1,0,0,0,1,1,1,1
                };
                worldSeed[i + 2] = newFace;
            }
        }
        for (int i = 0; i < 2; i++) //iterates through top and bottom sides of cube 
        {
            if (i == 0)
            {
                List<int> newFace = new List<int>
                {
                    1,1,0,0,0,0,0,1,
                    0,0,0,0,0,0,0,0,
                    0,0,0,1,0,0,0,0,
                    0,0,0,1,0,1,1,0,
                    1,0,0,1,0,1,1,0,
                    1,1,1,1,0,0,0,0,
                    1,0,0,0,0,0,0,1,
                    1,0,1,1,0,1,1,1
                };
                worldSeed[i + 4] = newFace;
            }
            if (i == 1)
            {
                List<int> newFace = new List<int>
                {
                    1,1,0,0,0,0,0,1,
                    1,0,0,0,0,0,0,0,
                    1,0,0,1,1,0,0,0,
                    1,0,0,1,1,1,0,0,
                    1,0,0,1,1,0,0,0,
                    1,0,0,0,0,0,0,0,
                    1,0,0,0,0,0,1,1,
                    1,1,1,0,1,1,1,1
                };
                worldSeed[i + 4] = newFace;
            }
        }
    }
    #endregion

    #region World Creation
    private void CreateWorld()
    {
        SeedToPositions();
        CreateGridAndBlocks();
    }
    #region Seed to Positions Translation
    private void SeedToPositions()
    {
        gridPositions = new List<VoxelPosition>{};
        blockPositions = new List<VoxelPosition>{};
        VoxelPosition newVoxel = new VoxelPosition{};
        for (int currentFace = 0; currentFace < worldSeed.Length; currentFace++)
        {
            if (currentFace == 0)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    currentColumn = i % worldSize[0];
                    if (currentColumn == 0)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = currentColumn + 1;
                    newVoxel.voxelPosition.y = currentRow - 1;
                    newVoxel.voxelPosition.z = 0;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 1)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    currentColumn = worldSize[0] - 1 - i % worldSize[0];
                    if (currentColumn == worldSize[0] - 1)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = currentColumn + 1;
                    newVoxel.voxelPosition.y = currentRow - 1;
                    newVoxel.voxelPosition.z = worldSize[2] + 1;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 2)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    currentColumn = worldSize[2] - 1 - i % worldSize[2];
                    if (currentColumn == worldSize[2] - 1)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = 0;
                    newVoxel.voxelPosition.y = currentRow - 1;
                    newVoxel.voxelPosition.z = currentColumn + 1;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 3)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    currentColumn = i % worldSize[2];
                    if (currentColumn == 0)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = worldSize[0] + 1;
                    newVoxel.voxelPosition.y = currentRow - 1;
                    newVoxel.voxelPosition.z = currentColumn + 1;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 4)
            {
                int currentRow = worldSize[2];
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    currentColumn = i % worldSize[0];
                    if (currentColumn == 0)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = currentColumn + 1;
                    newVoxel.voxelPosition.y = 0;
                    newVoxel.voxelPosition.z = currentRow + 1;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 5)
            {
                int currentRow = -1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    currentColumn = i % worldSize[0];
                    if (currentColumn == 0)
                    {
                        currentRow += 1;
                    }
                    newVoxel.voxelPosition.x = currentColumn + 1;
                    newVoxel.voxelPosition.y = -worldSize[1] - 1;
                    newVoxel.voxelPosition.z = currentRow + 1;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
        }
        for (int i = 0; i < blockPositions.Count(); i++)
        {
            VoxelPosition testVoxel;
            newVoxel.voxelPosition = blockPositions[i].voxelPosition;
            if (!blockPositions[i].voxelInPosition) newVoxel.voxelInPosition = true;
            else if (blockPositions[i].voxelInPosition) newVoxel.voxelInPosition = false;
            if (newVoxel.voxelPosition.x == 0)
            {
                newVoxel.voxelPosition.x += 1;
            }
            else if (newVoxel.voxelPosition.x == worldSize[0] + 1)
            {
                newVoxel.voxelPosition.x -= 1;
            }
            else if (newVoxel.voxelPosition.y == 0)
            {
                newVoxel.voxelPosition.y -= 1;
            }
            else if (newVoxel.voxelPosition.y == -worldSize[1] - 1)
            {
                newVoxel.voxelPosition.y += 1;
            }
            else if (newVoxel.voxelPosition.z == 0)
            {
                newVoxel.voxelPosition.z += 1;
            }
            else if (newVoxel.voxelPosition.z == worldSize[2] + 1)
            {
                newVoxel.voxelPosition.z -= 1;
            }
            testVoxel = newVoxel;
            testVoxel.voxelInPosition = !testVoxel.voxelInPosition;
            if (gridPositions.Contains(newVoxel) || gridPositions.Contains(testVoxel)) 
            {
                for (int j = 0; j < gridPositions.Count(); j++)
                {
                    if (gridPositions[j].voxelPosition == newVoxel.voxelPosition)
                    {
                        if (gridPositions[j].voxelInPosition == false && newVoxel.voxelInPosition == true)
                        {
                            gridPositions.Remove(gridPositions[j]);
                            gridPositions.Add(newVoxel);
                        }
                    }
                }
            }
            else
            {
                gridPositions.Add(newVoxel);
            }
        }
        for (int i = 0; i < blockPositions.Count(); i++)
        {
            if (blockPositions[i].voxelInPosition)
            {
                newVoxel = blockPositions[i];
                if (blockPositions[i].voxelPosition.x == 1)
                {
                    newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                    newVoxel.voxelPosition.x -= 1;
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                if (blockPositions[i].voxelPosition.x == worldSize[0])
                {
                    newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                    newVoxel.voxelPosition.x += 1;
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                if (blockPositions[i].voxelPosition.y == -1)
                {
                    newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                    newVoxel.voxelPosition.y += 1;
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                if (blockPositions[i].voxelPosition.y == -worldSize[1])
                {
                    newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                    newVoxel.voxelPosition.y -= 1;
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                if (blockPositions[i].voxelPosition.z == 1)
                {
                    newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                    newVoxel.voxelPosition.z -= 1;
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                if (blockPositions[i].voxelPosition.z == worldSize[2])
                {
                    newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                    newVoxel.voxelPosition.z += 1;
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
            }
        }
    }
    #endregion

    #region Grid and Block Creation 
    private void CreateGridAndBlocks()
    {
        float pixelSizeOffset = voxelSize / 2;
        float xPositionStart = -(worldSize[0] + 1) / 2 - pixelSizeOffset;
        float yPositionStart = (worldSize[1] + 1) / 2 + pixelSizeOffset;
        float zPositionStart = pixelSizeOffset + 8;
        float xPosition;
        float yPosition;
        float zPosition;
        for (int i = 0; i < gridPositions.Count(); i++)
        {
            if (gridPositions[i].voxelInPosition)
            {
                xPosition = xPositionStart + gridPositions[i].voxelPosition.x;
                yPosition = yPositionStart + gridPositions[i].voxelPosition.y;
                zPosition = zPositionStart + gridPositions[i].voxelPosition.z;
                Instantiate(gridVoxel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
            }
        }
        for (int i = 0; i < blockPositions.Count(); i++)
        {
            if (blockPositions[i].voxelInPosition)
            {
                xPosition = xPositionStart + blockPositions[i].voxelPosition.x;
                yPosition = yPositionStart + blockPositions[i].voxelPosition.y;
                zPosition = zPositionStart + blockPositions[i].voxelPosition.z;
                Instantiate(blockVoxel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
            }
        }
    }
    #endregion
    #endregion
}