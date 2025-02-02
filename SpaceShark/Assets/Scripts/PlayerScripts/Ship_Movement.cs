﻿using UnityEngine;
using System.Collections;

public class Ship_Movement : MonoBehaviour
{
    // Speed that the player is moving, accessible from other scripts
    public static float gameSpeed;
    // Restrict player from shooting in the tutorial
    public static bool restrictBullet = true;
    // The ship's current transform position, accessible from other scripts
    public static Vector3 shipPosition = new Vector3(0, 0, 0);
    // The current lane that the player is in
    public static LaneManager.LaneInfo currentLane = new LaneManager.LaneInfo();
    // The lane which the player is moving to
    public static LaneManager.LaneInfo targetLane = new LaneManager.LaneInfo();

    [Header("Game Object References")]
    [SerializeField]
    private LevelManager levelManager = null;
    //[SerializeField]
    //private GameObject goalObject = null;
    [SerializeField]
    private GameObject invincibilityObject = null;
    [SerializeField]
    private Material invincibilityMesh = null;
    [SerializeField]
    private GameObject sheildObject = null;


    [Header("Tutorial Settings")]
    [SerializeField]
    private float unrestrictedMovementDistance = 300.0f;

    //[SerializeField]
    // Reference to the sound manager in the scene
    private SoundManager soundManager;
    
    // How far the player is through the intro intro animation
    private float introProgress = 0.0f;
    // How long the intro animation lasts
    private float introDuration = 2.0f;
    // How far the player is through the level
    private float levelProgress;

    [SerializeField]
    // The starting speed of the player, and the speed at any point thereafter
    private float currentSpeed = 3.0f;
    [SerializeField]
    // How fast the player moves
    private float movementSpeed = 40.0f;

    // Restrict players movement in the tutorial
    private bool restrictSwipeVertical = true;
    private bool restrictSwipeHorizontal = true;
    private bool restrictSwipeDiagonal = true;

    private bool sheilded = false;
    private bool invincible = false;
    private bool alteredSpeed = false;
    private float invincibilityTimer = 0.0f;
    private float speedTimer = 0.0f;
    
    private StateManager state = null;

    //private AkSoundEngine wwise = new AkSoundEngine();

    // Set frame rate to 30
    void Awake ()
    {
        Application.targetFrameRate = 30;
        state = GameObject.Find("ScreenManager").GetComponent<StateManager>();
        soundManager = state.gameObject.GetComponent<SoundManager>();
    }

    // Use this for initialization
    void Start ()
    {
        // Initialiseinput
        GameInput.ResetSwipe();
        GameInput.OnSwipe += HandleOnSwipe;


        // Initialise ship
        currentLane = LaneManager.laneData[4];
        targetLane = currentLane;
        shipPosition = gameObject.transform.position;
        movementSpeed = GameSettings.gameSpeed;
        gameSpeed = currentSpeed;

        // Disable all player input at start of tutorial
        if (state.GetState() == StateManager.States.tutorial)
        {
            if (TutorialManager.shoot.enabled)
            {
                TutorialManager.shoot.enabled = false;
            }
            if (TutorialManager.horizontal.enabled)
            {
                TutorialManager.horizontal.enabled = false;
            }
            if (TutorialManager.vertical.enabled)
            {
                TutorialManager.vertical.enabled = false;
            }
            if (TutorialManager.diagonal.enabled)
            {
                TutorialManager.diagonal.enabled = false;
            }
        }

        // Start intro swoosh sound
        soundManager.GetComponent<SoundManager>().PlayEvent("shipEngine", gameObject);
        //soundManager.PlayEvent("menuSwipe", gameObject);
        sheildObject.SetActive(false);
        invincibilityObject.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        //state.SetToMenu();
        // If the game is in progress, move ship
        if (state.GetState() == StateManager.States.play)
        {
            // If the game has just started, do intro camera movement
            if (introProgress < introDuration)
            {
                introProgress += Time.deltaTime;
            }
            else
            {
                currentSpeed = movementSpeed;
            }

           

            // Move the player
            MoveToLane();
        
        // If the tutorial is in progress
       /* else if (state.GetState() == StateManager.States.tutorial)
        {
            // If the tutorial has just started, do intro camera movement
            if (introProgress < introDuration)
            {
                introProgress += Time.deltaTime;
                TutorialManager.DisableUI();
            }
            else
            {
                currentSpeed = movementSpeed;
            }

            // Remove input restricitions past the last helper UI
            if (shipPosition.z > unrestrictedMovementDistance)
            {
                restrictSwipeVertical = false;
                restrictSwipeHorizontal = false;
                restrictSwipeDiagonal = false;
                restrictBullet = false;
            }
            // Move the player
            MoveToLane();
        }*/

        if (invincible)
        {
            invincibilityTimer -= Time.deltaTime;
            float floor = 0.1f;
            float ceiling = 0.9f;
            float emission = floor + Mathf.PingPong (Time.time * 1.5f, ceiling - floor);
            Color baseColor = Color.yellow; //Replace this with whatever you want for your base color at emission level '1'
 
            Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);
 
            invincibilityMesh.SetColor ("_EmissionColor", finalColor);
            //invincibilityMesh.
            if (invincibilityTimer < 0)
            {
                invincibilityObject.SetActive(false);
                invincible = false;
                Debug.Log("Invincibility Finished");
            }
        }

        if (alteredSpeed)
        {
            speedTimer -= Time.deltaTime;
            if(speedTimer < 0)
            {
                soundManager.StopEvent("slowTime", 0, gameObject);
                soundManager.SetRTCP("GameSpeed", 1.0f);
                alteredSpeed = false;
                currentSpeed = GameSettings.gameSpeed;
                movementSpeed = GameSettings.gameSpeed;
            }
        }

        gameSpeed = currentSpeed;
        }
    }

    //If the player collides with an object, respond according to what the object was
    void OnTriggerEnter(Collider other)
    {
        // Player loses after hitting a bullet or wall
        if ((other.tag == "Obstacle") | (other.tag == "Laser"))
        {
            if (sheilded)
            {
                sheildObject.SetActive(false);
                sheilded = false;
                // stop a collision double firing - causing the bullet to still kill the player
                SetInvincible(0.05f);
            }
            else if (invincible)
            {
                
            }
            else
            {
                soundManager.PlayEvent("playerDeath", soundManager.gameObject);
                levelManager.ClearLevel();
            }
        }

        // Bring up tutorial Ui when the player hits the respective triggers
        if (state.GetState() == StateManager.States.tutorial)
        {
            if (other.tag == "HorizontalTutorial")
            {
                Debug.Log("horizontal enabled");
                movementSpeed = 0;
                restrictSwipeHorizontal = false;
                TutorialManager.horizontal.enabled = true;
            }
            else if (other.tag == "VerticalTutorial")
            {
                movementSpeed = 0;
                restrictSwipeVertical = false;
                TutorialManager.vertical.enabled = true;
            }
            else if (other.tag == "DiagonalTutorial")
            {
                movementSpeed = 0;
                restrictSwipeDiagonal = false;
                TutorialManager.diagonal.enabled = true;
            }
            else if (other.tag == "ShootingTutorial")
            {
                movementSpeed = 0;
                restrictSwipeVertical = false;
                restrictSwipeHorizontal = false;
                restrictSwipeDiagonal = false;
                restrictBullet = false;
                TutorialManager.shoot.enabled = true;
                GameInput.OnTap += HandleOnTap;
            }
        }
    }

    // For tutorial, when player taps after being shown shoot ui, unrestrict all movement
    private void HandleOnTap(Vector3 position)
    {
        movementSpeed = GameSettings.gameSpeed;
        TutorialManager.shoot.enabled = false;
        restrictSwipeHorizontal = false;
        restrictSwipeVertical = false;
        restrictSwipeDiagonal = false;
        // Shoot bullet down lane
        GameInput.OnTap -= HandleOnTap;
    }

    // Get the direction of any new swipe
    private void HandleOnSwipe(GameInput.Direction direction)
    {
        if (state.GetState() == StateManager.States.tutorial)
        {
            TutorialMovement(direction);
        }
        else if (state.GetState() == StateManager.States.play)
        {
            GameMovement(direction);
            currentLane = LaneManager.laneData[(int)targetLane.laneID];
        }
    }


    // Allow movement as long as it is not restricted
    private void TutorialMovement(GameInput.Direction direction)
    {
        if (introProgress > introDuration)
        {
            if (!restrictSwipeDiagonal & (!restrictSwipeHorizontal & !restrictSwipeVertical))
            {
                GameMovement(direction);
                currentLane = LaneManager.laneData[(int)targetLane.laneID];
                movementSpeed = GameSettings.gameSpeed;
            }
            else if (!restrictSwipeHorizontal)
            {
                if ((direction == GameInput.Direction.E) | (direction == GameInput.Direction.W))
                {
                    GameMovement(direction);
                    currentLane = LaneManager.laneData[(int)targetLane.laneID];
                    movementSpeed = GameSettings.gameSpeed;
                    restrictSwipeHorizontal = true;
                    TutorialManager.horizontal.enabled = false;
                }
            }
            else if (!restrictSwipeVertical)
            {
                if ((direction == GameInput.Direction.N) | (direction == GameInput.Direction.S))
                {
                    GameMovement(direction);
                    currentLane = LaneManager.laneData[(int)targetLane.laneID];
                    movementSpeed = GameSettings.gameSpeed;
                    restrictSwipeVertical = true;
                    TutorialManager.vertical.enabled = false;
                }
            }
            else if (!restrictSwipeDiagonal)
            {
                if (((direction == GameInput.Direction.NE) | (direction == GameInput.Direction.NW)) | ((direction == GameInput.Direction.SE) | (direction == GameInput.Direction.SW)))
                {
                    GameMovement(direction);
                    currentLane = LaneManager.laneData[(int)targetLane.laneID];
                    movementSpeed = GameSettings.gameSpeed;
                    restrictSwipeDiagonal = true;
                    TutorialManager.diagonal.enabled = false;
                }
            }
        }
    }

    // Set a new target lane based on player swipes, move player
    private void GameMovement(GameInput.Direction direction)
    {
        if (introProgress > introDuration)
        {
            soundManager.PlayEvent("playerSwipe", gameObject);
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
        }
    }

    // Moves the player forward and toward a different lane if necessary
    private void MoveToLane()
    {
        // Get current position
        shipPosition = gameObject.transform.position;
        shipPosition.z += (currentSpeed * Time.deltaTime);
        if ((shipPosition.x != targetLane.laneX) | (shipPosition.y != targetLane.laneY))
        {
            // Move toward the target position
            shipPosition.x = Mathf.SmoothStep(shipPosition.x, targetLane.laneX, Time.deltaTime * GameSettings.laneMoveSpeed);
            shipPosition.y = Mathf.SmoothStep(shipPosition.y, targetLane.laneY, Time.deltaTime * GameSettings.laneMoveSpeed);
        }
        // Set the updated position
        transform.position = shipPosition;
    } 

    public void SetSheild()
    {
        sheildObject.SetActive(true);
        sheilded = true;
    }

    public void SetInvincible(float duration)
    {
        invincibilityObject.SetActive(true);
        invincible = true;
        invincibilityTimer = duration;
    }

    public void ChangeSpeed(float speedMultiplier, float duration)
    {
       float maxSpeed = currentSpeed;
        currentSpeed = GameSettings.gameSpeed * speedMultiplier;
        movementSpeed = GameSettings.gameSpeed * speedMultiplier;
        float speedPercentage = movementSpeed / maxSpeed;
        soundManager.SetRTCP("GameSpeed", speedPercentage);
        alteredSpeed = true;
        speedTimer = duration;
    }
}
