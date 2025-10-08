using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{
    #region Dependencies
    [SerializeField] private GameObject worldPixel;
    #endregion
    #region Public Fields
    #endregion
    #region Private Variables
    [SerializeField] private float pixelSize = 1f;
    private GameObject[,] worldPixels;
    private int[] worldSize = { 8, 8, 8 };
    private List<int>[] worldSeed;
    #endregion

    #region Start
    void Start()
    {
        CreateSeed(worldSize[0], worldSize[1], worldSize[2]);
        CreateWorld(worldSize[0], worldSize[1], worldSize[2]);
    }
    #endregion

    #region Update
    void Update()
    {

    }
    #endregion

    #region Seed Creation
    private void CreateSeed(int x, int y, int z)
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
    private void CreateWorld(int x, int y, int z)
    {
        List<int>[] gridSeed = new List<int>[6];
        List<int>[] blockSeed = new List<int>[6];
        SeedToGrid(ref gridSeed, ref blockSeed);
        CreateGrid(x, y, z);
        CreateBlocks(blockSeed);
    }
    #region Seed to Grid Translation
    private void SeedToGrid(ref List<int>[] gridSeed, ref List<int>[] blockSeed) //do grid seed 
    {
        for (int currentFace = 0; currentFace < worldSeed.Length; currentFace++)
        {
            List<int> newGridFace = new List<int>{};
            List<int> newBlockFace = new List<int>{};
            int currentPixel = 0;
            int currentLine;
            int pixelsOnLine;
            if (currentFace == 0)
            {
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    if (i % worldSize[0] == 0)
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace + 2][i + worldSize[2] - 1] == 1)
                        {
                            if (i < worldSize[0])
                            {
                                if (worldSeed[currentFace + 4][i + worldSize[0] * (worldSize[2] - 1)] != 1)
                                {
                                    newGridFace.Add(0);
                                }
                            }
                            else
                            {
                                newGridFace.Add(1);
                            }
                        }
                    }
                    else if ((i + 1) % worldSize[0] == 0)
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace + 3][i - worldSize[2] + 1] == 1)
                        {
                            if (i < (worldSize[0] * worldSize[1]) && i > (worldSize[0] * (worldSize[1] - 1)))
                            {
                                if (worldSeed[currentFace + 5][i] != 1)
                                {
                                    newGridFace.Add(0);
                                }
                            }
                            else
                            {
                                newGridFace.Add(1);
                            }
                        }
                    }
                    else
                    {
                        newGridFace.Add(worldSeed[currentFace][i]);
                    }
                    newBlockFace.Add(worldSeed[currentFace][currentPixel]);
                    currentPixel += 1;
                }
            }
            else if (currentFace == 1)
            {
                currentLine = -1;
                pixelsOnLine = worldSize[0];
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    if (pixelsOnLine == worldSize[0])
                    {
                        pixelsOnLine = 0;
                        currentLine += 1;
                        currentPixel = currentLine * worldSize[0] + worldSize[0] - 1;
                    }
                    if (i % worldSize[0] == 0)
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace + 2][i + worldSize[2] - 1] == 1)
                        {
                            if (i < worldSize[0])
                            {
                                if (worldSeed[currentFace + 3][i + worldSize[0] * (worldSize[2] - 1)] != 1)
                                {
                                    newGridFace.Add(0);
                                }
                            }
                            else
                            {
                                newGridFace.Add(1);
                            }
                        }
                    }
                    else if ((i + 1) % worldSize[0] == 0)
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace + 1][i - worldSize[2] + 1] == 1)
                        {
                            if (i < (worldSize[0] * worldSize[1]) && i > (worldSize[0] * (worldSize[1] - 1)))
                            {
                                if (worldSeed[currentFace + 4][i] != 1)
                                {
                                    newGridFace.Add(0);
                                }
                            }
                            else
                            {
                                newGridFace.Add(1);
                            }
                        }
                    }
                    else
                    {
                        newGridFace.Add(worldSeed[currentFace][i]);
                    }
                    newBlockFace.Add(worldSeed[currentFace][currentPixel]);
                    pixelsOnLine += 1;
                    currentPixel -= 1;
                }
            }
            else if (currentFace == 2)
            {
                currentLine = -1;
                pixelsOnLine = worldSize[2];
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    if (pixelsOnLine == worldSize[2])
                    {
                        pixelsOnLine = 0;
                        currentLine += 1;
                        currentPixel = currentLine * worldSize[2] + worldSize[2] - 1;
                    }
                    if (i % worldSize[2] == 0 || (i + 1) % worldSize[2] == 0)
                    {
                        //be happy (don't add) 
                    }
                    else if (i < worldSize[2])
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace - 1][i + worldSize[2] - 1] == 1)
                        {
                            newGridFace.Add(1);
                        }
                    }
                    else if (i < (worldSize[2] * worldSize[1]) && i > (worldSize[2] * (worldSize[1] - 1)))
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace - 2][i - worldSize[2] + 1] == 1)
                        {
                            newGridFace.Add(1);
                        }
                    }
                    else
                    {
                        newGridFace.Add(worldSeed[currentFace][i]);
                    }
                    newBlockFace.Add(worldSeed[currentFace][currentPixel]);
                    pixelsOnLine += 1;
                    currentPixel -= 1;
                }
            }
            else if (currentFace == 3)
            {
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    if (i % worldSize[2] == 0 || (i + 1) % worldSize[2] == 0)
                    {
                        //be happy (don't add) 
                    }
                    else if (i < worldSize[2])
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace - 3][i + worldSize[2] - 1] == 1)
                        {
                            newGridFace.Add(1);
                        }
                    }
                    else if (i < (worldSize[2] * worldSize[1]) && i > (worldSize[2] * (worldSize[1] - 1)))
                    {
                        if (worldSeed[currentFace][i] == 1 && worldSeed[currentFace - 2][i - worldSize[2] + 1] == 1)
                        {
                            newGridFace.Add(1);
                        }
                    }
                    else
                    {
                        newGridFace.Add(worldSeed[currentFace][i]);
                    }
                    newBlockFace.Add(worldSeed[currentFace][currentPixel]);
                    currentPixel += 1;
                }
            }
            else if (currentFace == 4)
            {
                currentPixel = (worldSize[2] - 1) * worldSize[0] - 1;
                currentLine = -1;
                pixelsOnLine = worldSize[2];
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    if (pixelsOnLine == worldSize[2])
                    {
                        pixelsOnLine = 0;
                        currentLine += 1;
                        currentPixel = currentLine + (worldSize[2] - 1) * worldSize[0];
                    }
                    newBlockFace.Add(worldSeed[currentFace][currentPixel]);
                    pixelsOnLine += 1;
                    currentPixel -= worldSize[2];
                }
            }
            else if (currentFace == 5)
            {
                currentLine = -1;
                pixelsOnLine = worldSize[2];
                for (int i = 0; i < worldSeed[currentFace].Count(); i++)
                {
                    if (pixelsOnLine == worldSize[2])
                    {
                        pixelsOnLine = 0;
                        currentLine += 1;
                        currentPixel = currentLine;
                    }
                    //Debug.Log(currentPixel);
                    newBlockFace.Add(worldSeed[currentFace][currentPixel]);
                    pixelsOnLine += 1;
                    currentPixel += worldSize[2];
                }
            }
            gridSeed[currentFace] = newGridFace; 
            blockSeed[currentFace] = newBlockFace; 
        }
    }
    #endregion
    #region Grid Creation 
    private void CreateGrid(int x, int y, int z)
    {
        int currentFace = 0;
        int currentPixel = 0;
        float pixelSizeOffset = pixelSize / 2;
        float xPositionStart = -x / 2 - pixelSizeOffset;
        float yPositionStart = y / 2 + pixelSizeOffset;
        float zPositionStart = pixelSizeOffset;
        float xPosition = xPositionStart;
        float yPosition = yPositionStart;
        float zPosition = zPositionStart;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < x; k++)
                {
                    Instantiate(worldPixel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
                    xPosition += pixelSize;
                    currentPixel += 1;
                }
                xPosition = xPositionStart;
                yPosition -= pixelSize;
            } //front face created 
            xPosition = xPositionStart;
            yPosition = yPositionStart;
            zPosition += (z - 1) * pixelSize;
            currentPixel = 0;
            currentFace += 1;
        } //back face created 
        zPositionStart += pixelSize;
        xPosition = xPositionStart;
        yPosition = yPositionStart;
        zPosition = zPositionStart;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z - 2; k++)
                {
                    Instantiate(worldPixel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
                    zPosition += pixelSize;
                    currentPixel += 1;
                }
                zPosition = zPositionStart;
                yPosition -= pixelSize;
            } //left face created 
            zPosition = zPositionStart;
            yPosition = yPositionStart;
            xPosition += (x - 1) * pixelSize;
            currentPixel = 0;
            currentFace += 1;
        } //right face created 
        xPositionStart += pixelSize;
        xPosition = xPositionStart;
        yPosition = yPositionStart;
        zPosition = zPositionStart;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < x - 2; j++)
            {
                for (int k = 0; k < z - 2; k++)
                {
                    Instantiate(worldPixel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
                    zPosition += pixelSize;
                    currentPixel += 1;
                }
                zPosition = zPositionStart;
                xPosition += pixelSize;
            } //top face created 
            zPosition = zPositionStart;
            xPosition = xPositionStart;
            yPosition -= (y - 1) * pixelSize;
            currentPixel = 0;
            currentFace += 1;
        } //bottom face created 
    }
    #endregion
    #region Block Creation
    private void CreateBlocks(List<int>[] blockSeed)
    {
        for (int i = 0; i < blockSeed.Length; i++)
        {
            for (int j = 0; j < blockSeed[i].Count(); j++)
            {
                //Debug.Log(blockSeed[i][j]);
            }
        }
    }
    #endregion
    #endregion
}
