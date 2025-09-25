using UnityEngine;

public class WorldHandler : MonoBehaviour
{
    #region Dependencies
    [SerializeField] private GameObject worldPixel;
    #endregion
    #region Public Fields
    #endregion
    #region Private Variables
    [SerializeField] private float pixelSize = 1.0f;
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
    private void CreateWorld(int x, int y, int z){
        float xPositionStart = -x / 2 - pixelSize / 2;
        float yPositionStart = y / 2 + pixelSize / 2;
        float zPositionStart = 0f;
        float xPosition = xPositionStart;
        float yPosition = yPositionStart;
        float zPosition = zPositionStart;
        for (int i = 0; i < 2; i++){
            for (int j = 0; j < y; j++){
                for (int k = 0; k < x; k++){
                    Instantiate(worldPixel, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity, parent: this.transform);
                    xPosition += pixelSize;
                }
                xPosition = xPositionStart;
                yPosition -= pixelSize;
            }
            xPosition = xPositionStart;
            yPosition = yPositionStart;
            zPosition += z * pixelSize;
        }
    }
    #endregion
}
