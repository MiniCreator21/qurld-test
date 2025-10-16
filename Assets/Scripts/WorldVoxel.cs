using Unity.VisualScripting;
using UnityEngine;

public class WorldVoxel : MonoBehaviour
{
    #region Dependencies
    #endregion
    #region Public Fields
    public bool isPainted = false;
    #endregion
    #region Private Variables
    private Material material;
    #endregion

    #region Start
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }
    #endregion
    #region Initialise
    #endregion

    #region Update
    void Update()
    {
    }
    #endregion

    #region Collisions
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<Renderer>().material != null)
            {
                Material playerMaterial = collision.GetComponent<Renderer>().material;
                material.color = playerMaterial.color;
            }
            isPainted = true;
        }
    }
    #endregion
}
