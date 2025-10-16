using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WorldHandler : MonoBehaviour
{
    #region Dependencies
    [SerializeField] private GameObject gridVoxel;
    [SerializeField] private GameObject blockVoxel;
    #endregion
    #region Public Fields
    public Vector3 worldSize;
    public float voxelSize = 1f;
    public List<VoxelPosition> blockPositions;
    public bool worldCreated = false;
    #endregion
    #region Private Variables
    private List<int>[] worldSeed;
    private List<WorldVoxel> GridVoxels;
    private List<VoxelPosition> gridPositions;
    #endregion
    #region Voxel Struct
    [System.Serializable]
    public struct VoxelPosition
    {
        public bool voxelInPosition;
        public Vector3 voxelPosition;
    }
    #endregion

    #region Start
    void Start()
    {
        worldSize = Vector3.one * 8;
        UpdateDependencies();
        CreateSeed(worldSize);
        CreateWorld();
    }
    #endregion
    #region Initialise
    private void UpdateDependencies()
    {
        voxelSize = (gridVoxel.transform.localScale.x + gridVoxel.transform.localScale.y + gridVoxel.transform.localScale.z) / 3;
    }
    #endregion

    #region Update
    void Update()
    {
    }
    #endregion

    #region Seed Creation
    private void CreateSeed(Vector3 worldSize)
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
                    0,0,0,0,0,0,1,0,
                    0,1,0,0,0,0,0,0,
                    0,0,0,0,0,0,0,0,
                    0,0,1,0,0,0,0,0,
                    1,0,0,0,0,0,0,0,
                    0,0,0,1,0,0,0,0,
                    0,0,0,1,0,1,0,0,
                    0,0,0,1,0,1,0,1
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
                    1,0,0,0,0,0,0,1,
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
                    1,1,0,0,0,1,1,1,
                    1,0,0,0,0,0,0,0,
                    1,0,0,1,1,1,0,0,
                    1,0,0,1,1,0,0,0,
                    1,0,0,1,1,0,0,0,
                    1,1,0,0,0,0,0,0,
                    1,1,1,0,0,0,1,1,
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
        float pixelSizeOffset = voxelSize / 2;
        float xPositionStart = pixelSizeOffset - (worldSize.x / 2 + 1) * voxelSize;
        float yPositionStart = -pixelSizeOffset + (worldSize.y / 2 + 1) * voxelSize;
        float zPositionStart = pixelSizeOffset - (worldSize.z / 2 + 1) * voxelSize;
        for (int currentFace = 0; currentFace < worldSeed.Length; currentFace++)
        {
            if (currentFace == 0)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count; i++)
                {
                    currentColumn = i % (int)worldSize.x;
                    if (currentColumn == 0)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = xPositionStart + (currentColumn + 1) * voxelSize;
                    newVoxel.voxelPosition.y = yPositionStart + (currentRow - 1) * voxelSize;
                    newVoxel.voxelPosition.z = zPositionStart;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 1)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count; i++)
                {
                    currentColumn = (int)worldSize.x - 1 - i % (int)worldSize.x;
                    if (currentColumn == worldSize.x - 1)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = xPositionStart + (currentColumn + 1) * voxelSize;
                    newVoxel.voxelPosition.y = yPositionStart + (currentRow - 1) * voxelSize;
                    newVoxel.voxelPosition.z = zPositionStart + (worldSize.z + 1) * voxelSize;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 2)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count; i++)
                {
                    currentColumn = (int)worldSize.z - 1 - i % (int)worldSize.z;
                    if (currentColumn == worldSize.z - 1)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = xPositionStart;
                    newVoxel.voxelPosition.y = yPositionStart + (currentRow - 1) * voxelSize;
                    newVoxel.voxelPosition.z = zPositionStart + (currentColumn + 1) * voxelSize;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 3)
            {
                int currentRow = 1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count; i++)
                {
                    currentColumn = i % (int)worldSize.z;
                    if (currentColumn == 0)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = xPositionStart + (worldSize.x + 1) * voxelSize;
                    newVoxel.voxelPosition.y = yPositionStart + (currentRow - 1) * voxelSize;
                    newVoxel.voxelPosition.z = zPositionStart + (currentColumn + 1) * voxelSize;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 4)
            {
                int currentRow = (int)worldSize.z;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count; i++)
                {
                    currentColumn = i % (int)worldSize.x;
                    if (currentColumn == 0)
                    {
                        currentRow -= 1;
                    }
                    newVoxel.voxelPosition.x = xPositionStart + (currentColumn + 1) * voxelSize;
                    newVoxel.voxelPosition.y = yPositionStart;
                    newVoxel.voxelPosition.z = zPositionStart + (currentRow + 1) * voxelSize;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
            else if (currentFace == 5)
            {
                int currentRow = -1;
                int currentColumn;
                for (int i = 0; i < worldSeed[currentFace].Count; i++)
                {
                    currentColumn = i % (int)worldSize.x;
                    if (currentColumn == 0)
                    {
                        currentRow += 1;
                    }
                    newVoxel.voxelPosition.x = xPositionStart + (currentColumn + 1) * voxelSize;
                    newVoxel.voxelPosition.y = yPositionStart - (worldSize.y + 1) * voxelSize;
                    newVoxel.voxelPosition.z = zPositionStart + (currentRow + 1) * voxelSize;
                    if (worldSeed[currentFace][i] == 1) newVoxel.voxelInPosition = true;
                    if (worldSeed[currentFace][i] == 0) newVoxel.voxelInPosition = false;
                    blockPositions.Add(newVoxel);
                }
            }
        }
        for (int i = 0; i < blockPositions.Count; i++)
        {
            VoxelPosition testVoxel;
            newVoxel.voxelPosition = blockPositions[i].voxelPosition;
            if (!blockPositions[i].voxelInPosition) newVoxel.voxelInPosition = true;
            else if (blockPositions[i].voxelInPosition) newVoxel.voxelInPosition = false;
            if (newVoxel.voxelPosition.x == xPositionStart)
            {
                newVoxel.voxelPosition.x += voxelSize;
            }
            else if (newVoxel.voxelPosition.x == xPositionStart + worldSize.x + 1)
            {
                newVoxel.voxelPosition.x -= voxelSize;
            }
            else if (newVoxel.voxelPosition.y == yPositionStart)
            {
                newVoxel.voxelPosition.y -= voxelSize;
            }
            else if (newVoxel.voxelPosition.y == yPositionStart - worldSize.y - 1)
            {
                newVoxel.voxelPosition.y += voxelSize;
            }
            else if (newVoxel.voxelPosition.z == zPositionStart)
            {
                newVoxel.voxelPosition.z += voxelSize;
            }
            else if (newVoxel.voxelPosition.z == zPositionStart + worldSize.z + 1)
            {
                newVoxel.voxelPosition.z -= voxelSize;
            }
            testVoxel = newVoxel;
            testVoxel.voxelInPosition = !testVoxel.voxelInPosition;
            if (gridPositions.Contains(newVoxel) || gridPositions.Contains(testVoxel)) 
            {
                for (int j = 0; j < gridPositions.Count; j++)
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
        for (int i = 0; i < blockPositions.Count; i++)
        {
            VoxelPosition testVoxel;
            newVoxel = blockPositions[i];
            if (blockPositions[i].voxelPosition.x == xPositionStart + voxelSize)
            {
                newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                newVoxel.voxelPosition.x -= voxelSize;
                testVoxel = newVoxel;
                testVoxel.voxelInPosition = !newVoxel.voxelInPosition;
                if (blockPositions[i].voxelInPosition)
                {
                    if (blockPositions.Contains(testVoxel))
                    {
                        for (int j = 0; j < blockPositions.Count; j++)
                        {
                            if (blockPositions[j].voxelPosition == testVoxel.voxelPosition)
                            {
                                blockPositions.Remove(blockPositions[j]);
                            }
                        }
                    }
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                else
                {
                    if (!blockPositions.Contains(testVoxel)) blockPositions.Add(newVoxel);
                }
            }
            if (blockPositions[i].voxelPosition.x == xPositionStart + worldSize.x)
            {
                newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                newVoxel.voxelPosition.x += voxelSize;
                testVoxel = newVoxel;
                testVoxel.voxelInPosition = !newVoxel.voxelInPosition;
                if (blockPositions[i].voxelInPosition)
                {
                    if (blockPositions.Contains(testVoxel))
                    {
                        for (int j = 0; j < blockPositions.Count; j++)
                        {
                            if (blockPositions[j].voxelPosition == testVoxel.voxelPosition)
                            {
                                blockPositions.Remove(blockPositions[j]);
                            }
                        }
                    }
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                else
                {
                    if (!blockPositions.Contains(testVoxel)) blockPositions.Add(newVoxel);
                }
            }
            if (blockPositions[i].voxelPosition.y == yPositionStart - voxelSize)
            {
                newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                newVoxel.voxelPosition.y += voxelSize;
                testVoxel = newVoxel;
                testVoxel.voxelInPosition = !newVoxel.voxelInPosition;
                if (blockPositions[i].voxelInPosition)
                {
                    if (blockPositions.Contains(testVoxel))
                    {
                        for (int j = 0; j < blockPositions.Count; j++)
                        {
                            if (blockPositions[j].voxelPosition == testVoxel.voxelPosition)
                            {
                                blockPositions.Remove(blockPositions[j]);
                            }
                        }
                    }
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                else
                {
                    if (!blockPositions.Contains(testVoxel)) blockPositions.Add(newVoxel);
                }
            }
            if (blockPositions[i].voxelPosition.y == yPositionStart - worldSize.y)
            {
                newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                newVoxel.voxelPosition.y -= voxelSize;
                testVoxel = newVoxel;
                testVoxel.voxelInPosition = !newVoxel.voxelInPosition;
                if (blockPositions[i].voxelInPosition)
                {
                    if (blockPositions.Contains(testVoxel))
                    {
                        for (int j = 0; j < blockPositions.Count; j++)
                        {
                            if (blockPositions[j].voxelPosition == testVoxel.voxelPosition)
                            {
                                blockPositions.Remove(blockPositions[j]);
                            }
                        }
                    }
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                else
                {
                    if (!blockPositions.Contains(testVoxel)) blockPositions.Add(newVoxel);
                }
            }
            if (blockPositions[i].voxelPosition.z == zPositionStart + 1)
            {
                newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                newVoxel.voxelPosition.z -= voxelSize;
                testVoxel = newVoxel;
                testVoxel.voxelInPosition = !newVoxel.voxelInPosition;
                if (blockPositions[i].voxelInPosition)
                {
                    if (blockPositions.Contains(testVoxel))
                    {
                        for (int j = 0; j < blockPositions.Count; j++)
                        {
                            if (blockPositions[j].voxelPosition == testVoxel.voxelPosition)
                            {
                                blockPositions.Remove(blockPositions[j]);
                            }
                        }
                    }
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                else
                {
                    if (!blockPositions.Contains(testVoxel)) blockPositions.Add(newVoxel);
                }
            }
            if (blockPositions[i].voxelPosition.z == zPositionStart + worldSize.z)
            {
                newVoxel.voxelPosition = blockPositions[i].voxelPosition;
                newVoxel.voxelPosition.z += voxelSize;
                testVoxel = newVoxel;
                testVoxel.voxelInPosition = !newVoxel.voxelInPosition;
                if (blockPositions[i].voxelInPosition)
                {
                    if (blockPositions.Contains(testVoxel))
                    {
                        for (int j = 0; j < blockPositions.Count; j++)
                        {
                            if (blockPositions[j].voxelPosition == testVoxel.voxelPosition)
                            {
                                blockPositions.Remove(blockPositions[j]);
                            }
                        }
                    }
                    if (!blockPositions.Contains(newVoxel)) blockPositions.Add(newVoxel);
                }
                else
                {
                    if (!blockPositions.Contains(testVoxel)) blockPositions.Add(newVoxel);
                }
            }
        }
    }
    #endregion

    #region Grid and Block Creation 
    private void CreateGridAndBlocks()
    {
        GridVoxels = new List<WorldVoxel>{};
        float xPosition;
        float yPosition;
        float zPosition;
        for (int i = 0; i < gridPositions.Count; i++)
        {
            if (gridPositions[i].voxelInPosition)
            {
                GameObject worldVoxel;
                xPosition = gridPositions[i].voxelPosition.x;
                yPosition = gridPositions[i].voxelPosition.y;
                zPosition = gridPositions[i].voxelPosition.z;
                worldVoxel = Instantiate(gridVoxel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
                if (worldVoxel.GetComponent<WorldVoxel>() != null) GridVoxels.Add(worldVoxel.GetComponent<WorldVoxel>());
            }
        }
        for (int i = 0; i < blockPositions.Count; i++)
        {
            if (blockPositions[i].voxelInPosition)
            {
                xPosition = blockPositions[i].voxelPosition.x;
                yPosition = blockPositions[i].voxelPosition.y;
                zPosition = blockPositions[i].voxelPosition.z;
                Instantiate(blockVoxel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
            }
        }
        worldCreated = true;
    }
    #endregion
    #endregion
}