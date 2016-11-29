﻿using UnityEngine;
using System.Collections;

public class Ship_Movement : MonoBehaviour {

    private float transition = 0.0f;
    private float animationDuration = 2.0f;
    [SerializeField]
    private float setShipForwardSpeed = 40.0f;
    public float shipForwardSpeed = 1.0f;
    public static float gameSpeed;
    private bool restrictSwipe = true;
    static public bool restrictBullet = true;
    [SerializeField]
    private float shipMovementSpeed = 20.0f;

    [SerializeField]
    private LevelManager levelManager = null;

    private Transform shipTransform;
    public static Vector3 shipPosition = new Vector3(0, 0, 0);
    public static LaneManager.LaneInfo currentLane = new LaneManager.LaneInfo();
    public static LaneManager.LaneInfo targetLane = new LaneManager.LaneInfo();

    void Awake ()
    {
        Application.targetFrameRate = 30;
    }

    // Use this for initialization
    void Start () {
        currentLane = LaneManager.laneData[4];
        targetLane = currentLane;
        GameInput.OnTap += HandleOnTap;
        GameInput.OnSwipe += HandleOnSwipe;
        shipTransform = gameObject.GetComponent<Transform>();
        shipPosition = shipTransform.position;
        gameSpeed = shipForwardSpeed;
        StateManager.gameState = StateManager.States.play;
	}

    // Update is called once per frame
    void Update ()
    {
<<<<<<< HEAD
        if (StateManager.gameState == StateManager.States.play)
        {
            if (transition > 1.0f)
            {
                shipForwardSpeed = setShipForwardSpeed;
            }
            else
            {
                transition += Time.deltaTime * 1 / animationDuration;
            }

            MoveToLane();
        }
=======
        if (transition < 1.0f)
        {
            transition += Time.deltaTime * 1 / animationDuration;
        }
        else
        {
            shipForwardSpeed = setShipForwardSpeed;
        }

       if (shipPosition.z > 300)
        {
            restrictSwipe = false;
            restrictBullet = false;
        }
        MoveToLane();
>>>>>>> Level-Segments
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Obstacle") | (other.tag == "Laser"))
        {
            ResetShip();
            levelManager.ResetLevel();
        }

        if ((other.tag == "Collider")| (other.tag == "Laser"))
        {
            setShipForwardSpeed = 0;
            restrictSwipe = false;
        }
    }

    void ResetShip()
    {
        shipPosition = new Vector3 (0, 0, -1);
        transform.position = shipPosition;
        targetLane = LaneManager.laneData[4];
        currentLane = targetLane;
    }

    // Get the position of any new tap
    private void HandleOnTap(Vector3 position)
    {
        setShipForwardSpeed = 40;
        // Shoot bullet down lane
    }

    // Get the direction of any new swipe
    private void HandleOnSwipe(GameInput.Direction direction)
    {
        if (restrictSwipe == false)
        {
            switch (direction)
            {

                case GameInput.Direction.W:
                    //Move player left
                    if (((int)currentLane.laneID != 0) & (((int)currentLane.laneID != 3) & ((int)currentLane.laneID != 6)))
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID - 1)];
                    }
                    break;
                case GameInput.Direction.E:
                    //Move player Right
                    if (((int)currentLane.laneID != 2) & (((int)currentLane.laneID != 5) & ((int)currentLane.laneID != 8)))
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID + 1)];
                    }
                    break;
                case GameInput.Direction.N:
                    //Move player Up
                    if ((int)currentLane.laneID > 2)
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID - 3)];
                    }
                    break;
                case GameInput.Direction.S:
                    //Move player Down
                    if ((int)currentLane.laneID < 6)
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID + 3)];
                    }
                    break;
                case GameInput.Direction.NE:
                    //Move player up and right
                    if (((int)currentLane.laneID < 8) & (((int)currentLane.laneID > 2) & ((int)currentLane.laneID != 5)))
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID - 2)];
                    }
                    break;
                case GameInput.Direction.SE:
                    //Move player down and right
                    if (((int)currentLane.laneID < 5) & ((int)currentLane.laneID != 2))
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID + 4)];
                    }
                    break;
                case GameInput.Direction.SW:
                    //Move player down and left
                    if (((int)currentLane.laneID < 6) & (((int)currentLane.laneID > 0) & ((int)currentLane.laneID != 3)))
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID + 2)];
                    }
                    break;
                case GameInput.Direction.NW:
                    //Move player up and left
                    if (((int)currentLane.laneID > 3) & ((int)currentLane.laneID != 6))
                    {
                        targetLane = LaneManager.laneData[((int)currentLane.laneID - 4)];
                    }
                    break;

            }
            currentLane = LaneManager.laneData[(int)targetLane.laneID];
            setShipForwardSpeed = 40;

            if (shipPosition.z < 300)
            {
                restrictSwipe = true;
            }
        }
    }

    private void MoveToLane()
    {
      
        shipPosition = shipTransform.position;
        shipPosition.z += (shipForwardSpeed * Time.deltaTime);
        if ((shipPosition.x != targetLane.laneX) | (shipPosition.y != targetLane.laneY))
        {
            // Move toward the target position
            shipPosition.x = Mathf.SmoothStep(shipPosition.x, targetLane.laneX, Time.deltaTime * shipMovementSpeed);
            shipPosition.y = Mathf.SmoothStep(shipPosition.y, targetLane.laneY, Time.deltaTime * shipMovementSpeed);
        }
        // Set the updated position
        transform.position = shipPosition;
    } 

}
