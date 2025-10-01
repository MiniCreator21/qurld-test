using System.Collections.Generic;
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
    private int[] worldSize = {8,8,8};
    private List<List<int>> worldSeed;
    #endregion

    #region Start
    void Start()
    {
        CreateSeed();
        CreateWorld(worldSize[0], worldSize[1], worldSize[2]);
    }
    #endregion

    #region Update
    void Update()
    {
        
    }
    #endregion

    #region Seed Creation
    private void CreateSeed()
    {
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
            }
        }
        for (int i = 0; i < 2; i++) //iterates through left and right sides of cube 
        {
            if (i == 0)
            {
                List<int> newFace = new List<int>
                {
                    1,1,1,1,1,1,1,1,
                    0,0,0,0,0,0,0,1,
                    0,1,0,0,0,0,0,1,
                    0,0,0,1,1,0,1,0,
                    1,0,0,1,0,0,1,0,
                    0,0,0,0,0,1,1,0,
                    0,0,1,1,0,1,0,0,
                    1,1,1,1,1,1,0,0
                };
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
            }
        }
        for (int i = 0; i < 2; i++) //iterates through top and bottom sides of cube 
        {
            if (i == 0)
            {
                List<int> newFace = new List<int>
                {
                    1,0,1,1,0,1,1,1,
                    1,0,0,0,0,0,0,1,
                    1,1,1,1,0,0,0,0,
                    1,0,0,1,0,1,1,0,
                    0,0,0,1,0,1,1,0,
                    0,0,0,1,0,0,0,0,
                    0,0,0,0,0,0,0,0,
                    1,1,0,0,0,0,0,1
                };
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
            }
        }
    }
    #endregion

    #region World Creation
    private void CreateWorld(int x, int y, int z)
    {
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
                }
                xPosition = xPositionStart;
                yPosition -= pixelSize;
            } //front face created 
            xPosition = xPositionStart;
            yPosition = yPositionStart;
            zPosition += (z - 1) * pixelSize;
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
                }
                zPosition = zPositionStart;
                yPosition -= pixelSize;
            } //left face created 
            zPosition = zPositionStart;
            yPosition = yPositionStart;
            xPosition += (x - 1) * pixelSize;
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
                }
                zPosition = zPositionStart;
                xPosition += pixelSize;
            } //top face created 
            zPosition = zPositionStart;
            xPosition = xPositionStart;
            yPosition -= (y - 1) * pixelSize;
        } //bottom face created 
    }
    #endregion
}
