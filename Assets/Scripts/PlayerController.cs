using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    #region Dependencies
    [SerializeField] WorldHandler world;
    [SerializeField] new CameraController camera;
    #endregion
    #region Public Fields
    #endregion
    #region Private Variables
    private int[] worldSize;
    private float voxelSize;
    private bool initialised = false;
    enum PlayerState { Stationary, Moving, SwitchingSides }
    [SerializeField] private PlayerState playerState = PlayerState.Stationary;
    private Vector3 currentPosition;
    private int currentFace = 0;
    private Vector3 direction = Vector3.zero;
    private int spacesMoving = 0;
    [SerializeField] private float speed = 10f;
    private float snapDistance = 0.01f;
    private MovementInstruction[] movementInstructions;
    #endregion
    #region Movement Instructions Struct
    [System.Serializable]
    public struct MovementInstruction
    {
        public Vector3 left;
        public Vector3 right;
        public Vector3 up;
        public Vector3 down;
        public Vector3 toGrid;
    }
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
        movementInstructions = new MovementInstruction[6];
        MovementInstruction newInstruction = new MovementInstruction{};
        newInstruction.left = Vector3.left;
        newInstruction.right = Vector3.right;
        newInstruction.up = Vector3.up;
        newInstruction.down = Vector3.down;
        newInstruction.toGrid = Vector3.forward;
        movementInstructions[0] = newInstruction;
        newInstruction.left = Vector3.right;
        newInstruction.right = Vector3.left;
        newInstruction.up = Vector3.up;
        newInstruction.down = Vector3.down;
        newInstruction.toGrid = Vector3.back;
        movementInstructions[1] = newInstruction;
        newInstruction.left = Vector3.forward;
        newInstruction.right = Vector3.back;
        newInstruction.up = Vector3.up;
        newInstruction.down = Vector3.down;
        newInstruction.toGrid = Vector3.right;
        movementInstructions[2] = newInstruction;
        newInstruction.left = Vector3.back;
        newInstruction.right = Vector3.forward;
        newInstruction.up = Vector3.up;
        newInstruction.down = Vector3.down;
        newInstruction.toGrid = Vector3.left;
        movementInstructions[3] = newInstruction;
        newInstruction.left = Vector3.left;
        newInstruction.right = Vector3.right;
        newInstruction.up = Vector3.forward;
        newInstruction.down = Vector3.back;
        newInstruction.toGrid = Vector3.down;
        movementInstructions[4] = newInstruction;
        newInstruction.left = Vector3.left;
        newInstruction.right = Vector3.right;
        newInstruction.up = Vector3.back;
        newInstruction.down = Vector3.forward;
        newInstruction.toGrid = Vector3.up;
        movementInstructions[5] = newInstruction;
    }
    private void CheckToInitialise()
    {
        if (world.worldCreated)
        {
            InitialiseSnap();
            InitialiseSize();
            InitialisePosition();
            camera.InitialiseCameraPosition(worldSize[2], movementInstructions[currentFace].toGrid);
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
        Vector3 size = new Vector3(voxelSize - 0.1f * voxelSize, voxelSize - 0.1f * voxelSize, voxelSize - 0.1f * voxelSize);
        transform.localScale = size;
    }
    private void InitialisePosition()
    {
        worldSize = world.worldSize;
        float pixelSizeOffset = voxelSize / 2;
        float zPositionStart = pixelSizeOffset - (worldSize[2] / 2 + 1) * voxelSize;
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
        transform.position = currentPosition + movementInstructions[currentFace].toGrid * 0.1f;
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
                break;

            case PlayerState.Moving:
                transform.position += direction * speed * Time.deltaTime;
                CheckForDestination();
                break;
            case PlayerState.SwitchingSides:
                //do something
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
        if (playerState != PlayerState.Moving) Move(Vector2.left);
    }
    public void OnMoveRight()
    {
        if (playerState != PlayerState.Moving) Move(Vector2.right);
    }
    public void OnMoveUp()
    {
        if (playerState != PlayerState.Moving) Move(Vector2.up);
    }
    public void OnMoveDown()
    {
        if (playerState != PlayerState.Moving) Move(Vector2.down);
    }
    private void Move(Vector2 direction2D)
    {
        if (direction2D == Vector2.left)
        {
            direction = movementInstructions[currentFace].left;
        }
        else if (direction2D == Vector2.right)
        {
            direction = movementInstructions[currentFace].right;
        }
        else if (direction2D == Vector2.up)
        {
            direction = movementInstructions[currentFace].up;
        }
        else if (direction2D == Vector2.down)
        {
            direction = movementInstructions[currentFace].down;
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
        Vector3 vectorToDestination = currentPosition + direction * spacesMoving * voxelSize - transform.position + movementInstructions[currentFace].toGrid * 0.1f;
        float distanceToDestination = math.pow(vectorToDestination.x, 2) + math.pow(vectorToDestination.y, 2) + math.pow(vectorToDestination.z, 2);
        if (Mathf.Abs(distanceToDestination) < snapDistance)
        {
            currentPosition = currentPosition + direction * spacesMoving * voxelSize;
            transform.position = currentPosition + movementInstructions[currentFace].toGrid * 0.1f;
            direction = Vector3.zero;
            spacesMoving = 0;
            bool switchSides = CheckForSideSwitch();
            if (!switchSides) playerState = PlayerState.Stationary;
        }
    }

    private int CheckLine(Vector3 direction)
    {
        int spaces = 0;
        List<Vector2> positionsOnLine = new List<Vector2> { };
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
                    if (world.blockPositions[i].voxelPosition.y - currentPosition.y != 0)
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
        List<Vector2> linePositionsInOrder = new List<Vector2> { };
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

    #region Side Switch Checking
    private bool CheckForSideSwitch()
    {
        bool switchSides = false;
        float pixelSizeOffset = voxelSize / 2;
        float xEdge1 = pixelSizeOffset - (worldSize[0] / 2 + 1) * voxelSize;
        float xEdge2 = xEdge1 + (worldSize[0] + 1) * voxelSize;
        float yEdge1 = -pixelSizeOffset + (worldSize[1] / 2 + 1) * voxelSize;
        float yEdge2 = yEdge1 - (worldSize[1] + 1) * voxelSize;
        float zEdge1 = pixelSizeOffset + (worldSize[1] * 2f - worldSize[0] / 2 - 1) * voxelSize;
        float zEdge2 = zEdge1 + (worldSize[2] + 1) * voxelSize;
        return switchSides;
    }
    #endregion
    #endregion
}
