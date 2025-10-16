using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    #region Dependencies
    [SerializeField] WorldHandler world;
    [SerializeField] new CameraController camera;
    #endregion
    #region Public Fields
    public Vector3[,] movementInstructions;
    public int currentFace = 0;
    #endregion
    #region Private Variables
    private Vector3 worldSize;
    private float voxelSize;
    private bool initialised = false;
    enum PlayerState { Stationary, Moving, SwitchingFaces }
    [SerializeField] private PlayerState playerState = PlayerState.Stationary;
    private Vector3 currentPosition;
    private Vector2 direction2D = Vector2.zero;
    private Vector3 direction = Vector3.zero;
    private int spacesMoving = 0;
    [SerializeField] private float speed = 10f;
    private float snapDistance = 0.01f;
    #endregion

    #region Start
    void Start()
    {
        InitialiseMovementInstructions();
    }
    #endregion
    #region Initialise
    private void InitialiseMovementInstructions()
    {
        movementInstructions = new Vector3[6, 5];
        movementInstructions[0,0] = Vector3.up;
        movementInstructions[0,1] = Vector3.right;
        movementInstructions[0,2] = Vector3.down;
        movementInstructions[0,3] = Vector3.left;
        movementInstructions[0,4] = Vector3.forward;
        movementInstructions[1,0] = Vector3.up;
        movementInstructions[1,1] = Vector3.left;
        movementInstructions[1,2] = Vector3.down;
        movementInstructions[1,3] = Vector3.right;
        movementInstructions[1,4] = Vector3.back;
        movementInstructions[2,0] = Vector3.up;
        movementInstructions[2,1] = Vector3.back;
        movementInstructions[2,2] = Vector3.down;
        movementInstructions[2,3] = Vector3.forward;
        movementInstructions[2,4] = Vector3.right;
        movementInstructions[3,0] = Vector3.up;
        movementInstructions[3,1] = Vector3.forward;
        movementInstructions[3,2] = Vector3.down;
        movementInstructions[3,3] = Vector3.back;
        movementInstructions[3,4] = Vector3.left;
        movementInstructions[4,0] = Vector3.forward;
        movementInstructions[4,1] = Vector3.right;
        movementInstructions[4,2] = Vector3.back;
        movementInstructions[4,3] = Vector3.left;
        movementInstructions[4,4] = Vector3.down;
        movementInstructions[5,0] = Vector3.forward;
        movementInstructions[5,1] = Vector3.left;
        movementInstructions[5,2] = Vector3.back;
        movementInstructions[5,3] = Vector3.right;
        movementInstructions[5,4] = Vector3.up;
    }
    private void CheckToInitialise()
    {
        if (world.worldCreated)
        {
            InitialiseSnap();
            InitialiseSize();
            InitialisePosition();
            camera.InitialiseCameraPosition(movementInstructions[currentFace,4], (int)worldSize.z);
            initialised = true;
        }
    }
    private void InitialiseSnap()
    {
        snapDistance = 0.005f * speed;
    }
    private void InitialiseSize()
    {
        voxelSize = world.voxelSize;
        Vector3 size = new(voxelSize - 0.1f * voxelSize, voxelSize - 0.1f * voxelSize, voxelSize - 0.1f * voxelSize);
        transform.localScale = size;
    }
    private void InitialisePosition()
    {
        worldSize = world.worldSize;
        float pixelSizeOffset = voxelSize / 2;
        float zPositionStart = pixelSizeOffset - (worldSize.z / 2 + 1) * voxelSize;
        Vector3 closestBlockToCentre;
        closestBlockToCentre.x = 9999;
        closestBlockToCentre.y = 9999;
        closestBlockToCentre.z = 0;
        currentPosition = Vector3.zero;
        for (int i = 0; i < world.blockPositions.Count; i++)
        {
            if (world.blockPositions[i].voxelPosition.z == zPositionStart)
            {
                if (((Mathf.Abs(world.blockPositions[i].voxelPosition.x) + Mathf.Abs(world.blockPositions[i].voxelPosition.y)) / 2) <= ((Mathf.Abs(closestBlockToCentre.x) + Mathf.Abs(closestBlockToCentre.y)) / 2))
                {
                    closestBlockToCentre = world.blockPositions[i].voxelPosition;
                    if (!world.blockPositions[i].voxelInPosition) currentPosition = world.blockPositions[i].voxelPosition;
                }
            }
        }
        if (currentPosition == Vector3.zero)
        {
            for (int i = 0; i < world.blockPositions.Count; i++)
            {
                if (world.blockPositions[i].voxelPosition.z == zPositionStart && !world.blockPositions[i].voxelInPosition)
                {
                    currentPosition = world.blockPositions[i].voxelPosition;
                }
            }
        }
        currentFace = 0;
        transform.position = currentPosition + movementInstructions[currentFace,4] * 0.1f;
    }
    #endregion

    #region Update
    void Update()
    {
        if (!initialised) CheckToInitialise();
        HandleState();
    }
    #endregion

    #region State Machine
    private void HandleState()
    {
        switch (playerState)
        {
            case PlayerState.Stationary:
                direction2D = Vector2.zero;
                direction = Vector3.zero;
                break;

            case PlayerState.Moving:
                transform.position += direction * speed * Time.deltaTime;
                CheckForDestination();
                break;
            case PlayerState.SwitchingFaces:
                if (camera.faceSwitched)
                {
                    camera.faceSwitched = false;
                    Move();
                }
                break;
        }
    }
    #endregion

    #region Movement
    //just spaces to be able to see :)
    //yep
    //just a tad more 
    //uh huh
    //done :)
    #region Movement Input
    public void OnMoveLeft()
    {
        if (playerState == PlayerState.Stationary)
        {
            direction2D = Vector2.left;
            Move();
        }
    }
    public void OnMoveRight()
    {
        if (playerState == PlayerState.Stationary)
        {
            direction2D = Vector2.right;
            Move();
        }
    }
    public void OnMoveUp()
    {
        if (playerState == PlayerState.Stationary)
        {
            direction2D = Vector2.up;
            Move();
        }
    }
    public void OnMoveDown()
    {
        if (playerState == PlayerState.Stationary)
        {
            direction2D = Vector2.down;
            Move();
        }
    }
    private void Move()
    {
        if (direction2D == Vector2.zero) return;
        int rotation = 0;
        Vector3 cameraRotation = camera.transform.eulerAngles;
        //if (cameraRotation.x < 0) cameraRotation.x += 360;
        //if (cameraRotation.y < 0) cameraRotation.y += 360;
        //if (cameraRotation.z < 0) cameraRotation.z += 360;
        //if (cameraRotation.x >= 360) cameraRotation.x -= 360;
        //if (cameraRotation.y >= 360) cameraRotation.y -= 360;
        //if (cameraRotation.z >= 360) cameraRotation.z -= 360;
        Debug.Log(cameraRotation);
        for (int i = 0; i < 4; i++)
        {
            if (cameraRotation.z == i * 90)
            {
                rotation = i;
                Debug.Log("rotation = " + i);
            }
        }
        if (direction2D == Vector2.up)
        {
            if (0 + rotation >= 4) rotation -= 4;
            direction = movementInstructions[currentFace, 0 + rotation];
        }
        else if (direction2D == Vector2.right)
        {
            if (1 + rotation >= 4) rotation -= 4;
            direction = movementInstructions[currentFace, 1 + rotation];
        }
        else if (direction2D == Vector2.down)
        {
            if (2 + rotation >= 4) rotation -= 4;
            direction = movementInstructions[currentFace, 2 + rotation];
        }
        else if (direction2D == Vector2.left)
        {
            if (3 + rotation >= 4) rotation -= 4;
            direction = movementInstructions[currentFace, 3 + rotation];
        }
        spacesMoving = CheckLine(direction);
        if (spacesMoving == 0) direction = Vector3.zero;
        else if (spacesMoving > 0)
        {
            playerState = PlayerState.Moving;
        }
    }
    #endregion

    #region Movement Checks 
    private void CheckForDestination()
    {
        Vector3 vectorToDestination = currentPosition + direction * spacesMoving * voxelSize - transform.position + movementInstructions[currentFace,4] * 0.1f;
        float distanceToDestination = math.pow(vectorToDestination.x, 2) + math.pow(vectorToDestination.y, 2) + math.pow(vectorToDestination.z, 2);
        if (Mathf.Abs(distanceToDestination) < snapDistance)
        {
            currentPosition = currentPosition + direction * spacesMoving * voxelSize;
            transform.position = currentPosition + movementInstructions[currentFace,4] * 0.1f;
            bool switchSides = CheckForFaceSwitch();
            if (!switchSides)
            {
                direction = Vector3.zero;
                spacesMoving = 0;
                playerState = PlayerState.Stationary;
            }
        }
    }

    private int CheckLine(Vector3 direction)
    {
        int spaces = 0;
        List<Vector2> positionsOnLine = new(){};
        Vector2 newPosition = Vector2.zero;
        bool xLine = false;
        bool yLine = false;
        bool zLine = false;
        int sign = 1;
        if (direction.x != 0)
        {
            xLine = true;
            sign = (int)direction.x;
        }
        else if (direction.y != 0)
        {
            yLine = true;
            sign = (int)direction.y;
        }
        else if (direction.z != 0)
        {
            zLine = true;
            sign = (int)direction.z;
        }
        for (int i = 0; i < world.blockPositions.Count; i++)
        {
            if (xLine)
            {
                if (world.blockPositions[i].voxelPosition.y == currentPosition.y && world.blockPositions[i].voxelPosition.z == currentPosition.z)
                {
                    if (world.blockPositions[i].voxelPosition.x - currentPosition.x != 0)
                    {
                        if ((world.blockPositions[i].voxelPosition.x - currentPosition.x) * sign > 0)
                        {
                            newPosition.x = world.blockPositions[i].voxelPosition.x;
                            if (world.blockPositions[i].voxelInPosition) newPosition.y = 1;
                            else if (!world.blockPositions[i].voxelInPosition) newPosition.y = 0;
                            positionsOnLine.Add(newPosition);
                        }
                    }
                }
            }
            else if (yLine)
            {
                if (world.blockPositions[i].voxelPosition.x == currentPosition.x && world.blockPositions[i].voxelPosition.z == currentPosition.z)
                {
                    if (world.blockPositions[i].voxelPosition.y - currentPosition.y != 0)
                    {
                        if ((world.blockPositions[i].voxelPosition.y - currentPosition.y) * sign > 0)
                        {
                            newPosition.x = world.blockPositions[i].voxelPosition.y;
                            if (world.blockPositions[i].voxelInPosition) newPosition.y = 1;
                            else if (!world.blockPositions[i].voxelInPosition) newPosition.y = 0;
                            positionsOnLine.Add(newPosition);
                        }
                    }
                }
            }
            else if (zLine)
            {
                if (world.blockPositions[i].voxelPosition.x == currentPosition.x && world.blockPositions[i].voxelPosition.y == currentPosition.y)
                {
                    if (world.blockPositions[i].voxelPosition.z - currentPosition.z != 0)
                    {
                        if ((world.blockPositions[i].voxelPosition.z - currentPosition.z) * sign > 0)
                        {
                            newPosition.x = world.blockPositions[i].voxelPosition.z;
                            if (world.blockPositions[i].voxelInPosition) newPosition.y = 1;
                            else if (!world.blockPositions[i].voxelInPosition) newPosition.y = 0;
                            positionsOnLine.Add(newPosition);
                        }
                    }
                }
            }
        }
        List<Vector2> linePositionsInOrder = new(){};
        for (int i = 0; i < positionsOnLine.Count; i++)
        {
            Vector2 smallestDifference;
            smallestDifference.x = 9999f;
            smallestDifference.y = 1;
            int indexOfSmallestDifference = -1;
            for (int j = 0; j < positionsOnLine.Count; j++)
            {
                bool addDifference = false;
                if (xLine && Mathf.Abs(positionsOnLine[j].x - currentPosition.x) < Mathf.Abs(smallestDifference.x - currentPosition.x))
                {
                    if (linePositionsInOrder.Count > 0)
                    {
                        if (Mathf.Abs(positionsOnLine[j].x - currentPosition.x) > Mathf.Abs(linePositionsInOrder[linePositionsInOrder.Count - 1].x - currentPosition.x))
                        {
                            addDifference = true;
                        }
                    }
                    else
                    {
                        addDifference = true;
                    }
                }
                else if (yLine && Mathf.Abs(positionsOnLine[j].x - currentPosition.y) < Mathf.Abs(smallestDifference.x - currentPosition.y))
                {
                    if (linePositionsInOrder.Count > 0)
                    {
                        if (Mathf.Abs(positionsOnLine[j].x - currentPosition.y) > Mathf.Abs(linePositionsInOrder[linePositionsInOrder.Count - 1].x - currentPosition.y))
                        {
                            addDifference = true;
                        }
                    }
                    else
                    {
                        addDifference = true;
                    }
                }
                else if (zLine && Mathf.Abs(positionsOnLine[j].x - currentPosition.z) < Mathf.Abs(smallestDifference.x - currentPosition.z))
                {
                    if (linePositionsInOrder.Count > 0)
                    {
                        if (Mathf.Abs(positionsOnLine[j].x - currentPosition.z) > Mathf.Abs(linePositionsInOrder[linePositionsInOrder.Count - 1].x - currentPosition.z))
                        {
                            addDifference = true;
                        }
                    }
                    else
                    {
                        addDifference = true;
                    }
                }
                if (addDifference)
                {
                    smallestDifference = positionsOnLine[j];
                    indexOfSmallestDifference = j;
                }
            }
            if (indexOfSmallestDifference != -1)
            {
                linePositionsInOrder.Add(smallestDifference);
            }
        }
        bool blockHit = false;
        for (int i = 0; i < linePositionsInOrder.Count; i++)
        {
            if (!blockHit && linePositionsInOrder[i].y == 1)
            {
                spaces = i;
                blockHit = true;
            }
        }
        if (!blockHit) spaces = linePositionsInOrder.Count;
        return spaces;
    }
    #endregion

    #region Face Switch Checking
    private bool CheckForFaceSwitch()
    {
        bool switchFaces = false;
        int lastFace = 0;
        float pixelSizeOffset = voxelSize / 2;
        float xEdge1 = pixelSizeOffset - (worldSize.x / 2 + 1) * voxelSize;
        float xEdge2 = xEdge1 + (worldSize.x + 1) * voxelSize;
        float yEdge1 = -pixelSizeOffset + (worldSize.y / 2 + 1) * voxelSize;
        float yEdge2 = yEdge1 - (worldSize.y + 1) * voxelSize;
        float zEdge1 = pixelSizeOffset - (worldSize.z / 2 + 1) * voxelSize;
        float zEdge2 = zEdge1 + (worldSize.z + 1) * voxelSize;
        if (currentPosition.x == xEdge1 && currentPosition.z == zEdge1 || currentPosition.x == xEdge2 && currentPosition.z == zEdge1 || currentPosition.y == yEdge1 && currentPosition.z == zEdge1 || currentPosition.y == yEdge2 && currentPosition.z == zEdge1 || currentPosition.x == xEdge1 && currentPosition.z == zEdge2 || currentPosition.x == xEdge2 && currentPosition.z == zEdge2 || currentPosition.y == yEdge1 && currentPosition.z == zEdge2 || currentPosition.y == yEdge2 && currentPosition.z == zEdge2 || currentPosition.x == xEdge1 && currentPosition.y == yEdge1 || currentPosition.x == xEdge2 && currentPosition.y == yEdge1 || currentPosition.x == xEdge1 && currentPosition.y == yEdge2 || currentPosition.x == xEdge2 && currentPosition.y == yEdge2)
        {
            lastFace = currentFace;
            if (currentFace == 0)
            {
                if (direction == Vector3.left) currentFace = 2;
                else if (direction == Vector3.right) currentFace = 3;
                else if (direction == Vector3.up) currentFace = 4;
                else if (direction == Vector3.down) currentFace = 5;
            }
            else if (currentFace == 1)
            {
                if (direction == Vector3.right) currentFace = 3;
                else if (direction == Vector3.left) currentFace = 2;
                else if (direction == Vector3.up) currentFace = 4;
                else if (direction == Vector3.down) currentFace = 5;
            }
            else if (currentFace == 2)
            {
                if (direction == Vector3.forward) currentFace = 1;
                else if (direction == Vector3.back) currentFace = 0;
                else if (direction == Vector3.up) currentFace = 4;
                else if (direction == Vector3.down) currentFace = 5;
            }
            else if (currentFace == 3)
            {
                if (direction == Vector3.back) currentFace = 0;
                else if (direction == Vector3.forward) currentFace = 1;
                else if (direction == Vector3.up) currentFace = 4;
                else if (direction == Vector3.down) currentFace = 5;
            }
            else if (currentFace == 4)
            {
                if (direction == Vector3.left) currentFace = 2;
                else if (direction == Vector3.right) currentFace = 3;
                else if (direction == Vector3.forward) currentFace = 1;
                else if (direction == Vector3.back) currentFace = 0;
            }
            else if (currentFace == 5)
            {
                if (direction == Vector3.left) currentFace = 2;
                else if (direction == Vector3.right) currentFace = 3;
                else if (direction == Vector3.back) currentFace = 0;
                else if (direction == Vector3.forward) currentFace = 1;
            }
            playerState = PlayerState.SwitchingFaces;
            switchFaces = true;
            camera.SwitchFace(direction2D);
            transform.position = currentPosition + movementInstructions[currentFace,4] * 0.1f;
        }
        return switchFaces;
    }
    #endregion
    #endregion
}
