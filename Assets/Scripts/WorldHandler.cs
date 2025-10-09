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
    private void CreateWorld(int x, int y, int z)
    {
        List<int>[] gridSeed = new List<int>[6];
        List<int>[] blockSeed = new List<int>[6];
        SeedToGrid(ref gridSeed, ref blockSeed);
        CreateGrid(x, y, z, gridSeed);
        CreateBlocks(blockSeed);
    }
    #region Seed to Grid Translation
    private void SeedToGrid(ref List<int>[] gridSeed, ref List<int>[] blockSeed)
    {
        for (int currentFace = 0; currentFace < worldSeed.Length; currentFace++)
        {
            List<int> newGridFace = new List<int> { };
            List<int> newBlockFace = new List<int> { };
            if (currentFace == 0)
            {
                newBlockFace = CopyList(worldSeed[currentFace]);
            }
            else if (currentFace == 1)
            {
                newBlockFace = ReverseListHorizontal(worldSeed[currentFace], worldSize[0]);
            }
            else if (currentFace == 2)
            {
                newBlockFace = ReverseListHorizontal(worldSeed[currentFace], worldSize[2]);
            }
            else if (currentFace == 3)
            {
                newBlockFace = CopyList(worldSeed[currentFace]);
            }
            else if (currentFace == 4)
            {
                newBlockFace = CopyList(worldSeed[currentFace]);
                newBlockFace = ReverseListVertical(newBlockFace, worldSize[0], worldSize[2]);
            }
            else if (currentFace == 5)
            {
                newBlockFace = CopyList(worldSeed[currentFace]);
            }
            gridSeed[currentFace] = newGridFace;
            blockSeed[currentFace] = newBlockFace;
        }
    }

    private List<int> CopyList(List<int> toCopy)
    {
        List<int> newCopy = new List<int> { };
        for (int i = 0; i < toCopy.Count(); i++)
        {
            newCopy.Add(toCopy[i]);
        }
        return newCopy;
    }
    private List<int> ReverseListHorizontal(List<int> toReverse, int horizontalSize)
    {
        List<int> newList = new List<int> { };
        int currentLine = -1;
        int pixelsOnLine;
        for (int i = 0; i < toReverse.Count(); i++)
        {
            pixelsOnLine = i % horizontalSize;
            if (pixelsOnLine == 0)
            {
                currentLine += 1;
            }
            newList.Add(toReverse[(currentLine + 1) * horizontalSize - 1 - pixelsOnLine]);
        }
        return newList;
    }
    private List<int> ReverseListVertical(List<int> toReverse, int horizontalSize, int verticalSize)
    {
        List<int> newList = new List<int> { };
        int currentLine = verticalSize;
        int pixelsOnLine;
        for (int i = 0; i < toReverse.Count(); i++)
        {
            pixelsOnLine = i % horizontalSize;
            if (pixelsOnLine == 0)
            {
                currentLine -= 1;
            }
            newList.Add(currentLine * horizontalSize + pixelsOnLine);
        }
        return newList;
    }
    #endregion

    #region Grid Creation 
    private void CreateGrid(int x, int y, int z, List<int>[] gridSeed)
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
                    //if (gridSeed[currentFace][j * worldSize[0] + k] != 1) 
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
                    //if (gridSeed[currentFace][j * worldSize[2] + k] != 1) 
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
                    //if (gridSeed[currentFace][j * worldSize[0] + k] != 1) 
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
