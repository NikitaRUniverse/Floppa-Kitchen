using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed of movement animation
    private Vector2Int currentGridPosition;

    private bool isMoving = false;  // To prevent multiple inputs during movement
    private bool startedMoving = false;
    private GameObject[,] grid;     // Reference to the map grid for checking obstacles
    private MapGenerator mapGenerator;
    public DarkPlayer darkPlayer;
    public int weapon = 0;
    public int swords = 9;
    public int fireSwords = 9;
    public int lives = 9;
    public int max_lives = 9;
    public int keys = 0;
    public int max_keys;
    public bool isWon = false;
    public bool isLost = false;
    public bool canMove = false;
    private ImageSwitcher imageSwitcher;
    private TextMeshProUGUI dumplingsT;
    private TextMeshProUGUI livesT;
    private TextMeshProUGUI pawsT;
    private TextMeshProUGUI belliesT;

    void Start()
    {
        dumplingsT = GameObject.FindGameObjectWithTag("UI 1").GetComponent<TextMeshProUGUI>();
        livesT = GameObject.FindGameObjectWithTag("UI 2").GetComponent<TextMeshProUGUI>();
        pawsT = GameObject.FindGameObjectWithTag("UI 3").GetComponent<TextMeshProUGUI>();
        belliesT = GameObject.FindGameObjectWithTag("UI 4").GetComponent<TextMeshProUGUI>();

        // Find the map generator and get the grid
        mapGenerator = FindObjectOfType<MapGenerator>();
        imageSwitcher = FindObjectOfType<ImageSwitcher>();
        max_keys = mapGenerator.keyCount;
        grid = mapGenerator.grid;

        // Set initial grid position for the player
        currentGridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        transform.position = new Vector3(currentGridPosition.x, currentGridPosition.y, 0);  // Snap player to grid position

        darkPlayer = GameObject.FindGameObjectWithTag("Dark").GetComponent<DarkPlayer>();
        GameObject.FindGameObjectWithTag("Dark").transform.position = transform.position;
        StartCoroutine(Starting(3.0f));
    }

    private IEnumerator Starting(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }

    void Update()
    {
        dumplingsT.text = ("Dumplings: " + keys + "/" + max_keys);
        livesT.text = ("Lives: " + lives + "/" + max_lives);
        pawsT.text = ("Paw Attacks: " + swords);
        belliesT.text = ("Belly Attacks: " + fireSwords);
        darkPlayer.TrackPlayerPosition(transform.position);

        if (!isWon && !isLost && canMove)
        {

            if (!isMoving)
            {
                Vector2Int direction = Vector2Int.zero;

                // Check for arrow key input
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    direction = Vector2Int.up;
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    direction = Vector2Int.down;
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    direction = Vector2Int.left;
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    direction = Vector2Int.right;

                // Attempt to move in the chosen direction
                if (direction != Vector2Int.zero)
                {
                    Vector2Int targetPosition = currentGridPosition + direction;
                    TryMoveTo(targetPosition);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Z)) {
                weapon = 0;
                imageSwitcher.StopEndlessGif();
            }
            else if (Input.GetKeyDown(KeyCode.X) && swords > 0) { 
                weapon = 1;
                imageSwitcher.StopEndlessGif();
                imageSwitcher.PlayGif("Sword", true);
            }
            else if (Input.GetKeyDown(KeyCode.C) && fireSwords > 0) { 
                weapon = 2;
                imageSwitcher.StopEndlessGif();
                imageSwitcher.PlayGif("Fire Sword", true);
            }

            if (swords <= 0 && weapon == 1)
            {
                imageSwitcher.StopEndlessGif();
                weapon = 0;
            }

            else if (fireSwords <= 0 && weapon == 2)
            {
                imageSwitcher.StopEndlessGif();
                weapon = 0;
            }
        }

        if (keys >= max_keys && !isWon && !isLost)
        {
            isWon = true;
            Invoke("RedirectToWinScene", 3.0f);
        }

        else if (lives <= 0 && !isWon && !isLost)
        {
            isLost = true;
            GetComponent<SpriteRenderer>().enabled = false;
            Invoke("RedirectToLoseScene", 3.0f);
        }
    }

    void RedirectToWinScene()
    {
        SceneManager.LoadScene("Win");
    }

    void RedirectToLoseScene()
    {
        SceneManager.LoadScene("Lost");
    }

    void TryMoveTo(Vector2Int targetPosition)
    {
        if (targetPosition.x >= 0 && targetPosition.x < mapGenerator.gridSize &&
            targetPosition.y >= 0 && targetPosition.y < mapGenerator.gridSize)
        {
            GameObject targetCell = grid[targetPosition.x, targetPosition.y];

            if (weapon == 0)
            {
                if (targetCell != null && !targetCell.CompareTag("Obstacle") && !targetCell.CompareTag("Enemy"))
                {
                    if (targetCell.CompareTag("Floor"))
                    {
                        targetCell.GetComponent<floor>().unlocking();
                    }

                    StartCoroutine(MoveToCell(targetPosition));
                }
                else if (targetCell.CompareTag("Enemy"))
                {
                    targetCell.GetComponent<enemy>().unlocking();
                    imageSwitcher.PlayGif("Enemy", false);

                    if (targetCell.GetComponent<enemy>().Is2())
                    {
                        lives--;
                    }
                    else
                    {
                        lives -= 2;
                    }

                    if (lives < 0)
                    {
                        lives = 0;
                    }
                }
                else if (targetCell.CompareTag("Obstacle"))
                {
                    targetCell.GetComponent<obstacle>().unlocking();
                    imageSwitcher.PlayGif("Wall", false);
                }
            }

            else if (weapon == 1)
            {
                if (targetCell != null)
                {
                    if (targetCell.CompareTag("Floor"))
                    {
                        targetCell.GetComponent<floor>().unlocking();
                    }

                    StartCoroutine(MoveToCell(targetPosition));
                    swords--;

                    if (targetCell.CompareTag("Obstacle") || targetCell.CompareTag("Enemy"))
                    {
                        targetCell.tag = "FakeFloor";
                        targetCell.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }

            else if (weapon == 2)
            {
                if (targetCell != null)
                {
                    if (targetCell.CompareTag("Floor"))
                    {
                        targetCell.GetComponent<floor>().unlocking();
                    }
                    StartCoroutine(MoveToCell(targetPosition));
                    fireSwords--;

                    if (targetCell.CompareTag("Obstacle") || targetCell.CompareTag("Enemy"))
                    {
                        targetCell.tag = "FakeFloor";
                        targetCell.GetComponent<SpriteRenderer>().enabled = false;
                    }

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue; // Skip the target cell itself

                            int neighborX = targetPosition.x + dx;
                            int neighborY = targetPosition.y + dy;

                            if (neighborX >= 0 && neighborX < mapGenerator.gridSize &&
                                neighborY >= 0 && neighborY < mapGenerator.gridSize)
                            {
                                GameObject neighborCell = grid[neighborX, neighborY];
                                if (neighborCell != null && (neighborCell.CompareTag("Obstacle") || neighborCell.CompareTag("Enemy")))
                                {
                                    neighborCell.tag = "FakeFloor";
                                    neighborCell.GetComponent<SpriteRenderer>().enabled = false;
                                }

                                else if (neighborCell != null && (neighborCell.CompareTag("Key")))
                                {

                                    neighborCell.GetComponent<key>().unlocking();
                                }

                                else if (neighborCell != null && (neighborCell.CompareTag("Floor")))
                                {
                                    neighborCell.GetComponent<floor>().unlocking();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    System.Collections.IEnumerator MoveToCell(Vector2Int targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, 0);
        float t = 0;

        // Move smoothly to the target cell
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // Snap to the final position and update current grid position
        transform.position = endPosition;
        currentGridPosition = targetPosition;

        startedMoving = true;
        isMoving = false;  // Allow new movement input
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            Destroy(other.gameObject);
            keys++;
            if (lives < max_lives)
            {
                lives++;
            }
            imageSwitcher.PlayGif("Key Found", false);
        }

        else if (other.CompareTag("Dark") && !isWon && !isLost)
        {
            if (startedMoving)
            {
                lives = 0;
            }
        }

        else if (other.CompareTag("Floor"))
        {
            if (!startedMoving)
            {
                other.GetComponent<floor>().unlocking();
                other.GetComponent<floor>().Starter();
            }
        }
    }
}
