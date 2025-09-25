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
    #endregion

    #region Start
    void Start()
    {
        CreateWorld(worldSize[0], worldSize[1], worldSize[2]);
    }
    #endregion

    #region Update
    void Update()
    {
        
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
            }
            xPosition = xPositionStart;
            yPosition = yPositionStart;
            zPosition += (z - 1) * pixelSize;
        }
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
            }
            zPosition = zPositionStart;
            yPosition = yPositionStart;
            xPosition += (x - 1) * pixelSize;
        }
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
            }
            zPosition = zPositionStart;
            xPosition = xPositionStart;
            yPosition -= (y - 1) * pixelSize;
        }
    }
    #endregion
}
