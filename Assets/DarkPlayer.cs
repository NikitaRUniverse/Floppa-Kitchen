using UnityEngine;
using System.Collections.Generic;

public class DarkPlayer : MonoBehaviour
{
    private Queue<Vector2> playerPositions; // Queue to store the player’s previous positions
    private Vector2 currentTarget;        // The current position the DarkPlayer is moving towards

    private float startTime;
    private bool canMove = false;         // Whether the player can start moving
    public PlayerController playerController;

    void Start()
    {
        playerPositions = new Queue<Vector2>();
        currentTarget = transform.position;
        startTime = Time.time;
    }

    // Call this function every frame from the PlayerController to update the DarkPlayer's path
    public void TrackPlayerPosition(Vector2 playerPosition)
    {
        playerPositions.Enqueue(playerPosition);
    }

    void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (!playerController.isWon && !playerController.isLost)
        {
            // Check if 3 seconds have passed since the game started
            if (Time.time - startTime >= 2f)
            {
                // Enable movement after 3 seconds
                canMove = true;
            }

            // If movement is allowed and there's a position to move to, dequeue and move
            if (canMove && playerPositions.Count > 0)
            {
                // Dequeue the next position
                currentTarget = playerPositions.Dequeue();

                // Move to the target position
                MoveToPosition(currentTarget);
            }
        }

        else if (playerController.isWon)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void MoveToPosition(Vector2 targetPosition)
    {
        // Move directly to the target position (instant movement)
        transform.position = targetPosition;
    }
}
