using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveDelay = 0.2f;
    [SerializeField] private float gridSize = 1f;

    [Header("Body")]
    [SerializeField] private GameObject bodyPrefab;

    [Header("Food")]
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Vector2Int gridMin = new Vector2Int(-8, -4);
    [SerializeField] private Vector2Int gridMax = new Vector2Int(8, 4);

    private GameInputActions input;
    private Vector2 inputDirection;
    private Vector2 moveDirection = Vector2.right;

    private float moveTimer;

    private readonly List<Transform> bodySegments = new();
    private readonly List<Vector3> positions = new();

    private Transform foodTransform;

    private int score;
    private bool isGameOver;

    [SerializeField] private float minMoveDelay = 0.08f;
    [SerializeField] private float speedUpAmount = 0.005f;
    [SerializeField] private GamePlayUIController gamePlayUIController;
    
    private void Awake()
    {
        input = new GameInputActions();
    }

    private void Start()
    {
        if (gamePlayUIController != null)
        {
            gamePlayUIController.SetScore(score);
        }
        Grow();
        SpawnFood();
    }

    private void OnEnable()
    {
        input.Enable();
        input.gamePlay.Move.performed += OnMove;
        input.gamePlay.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        input.gamePlay.Move.performed -= OnMove;
        input.gamePlay.Move.canceled -= OnMove;
        input.Disable();
    }

    private void Update()
    {
        if (isGameOver)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                Debug.Log("R pressed");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Debug.Log("ESC pressed");
                SceneManager.LoadScene("mainMenu");
            }

            return;
        }

        moveTimer += Time.deltaTime;

        if (moveTimer >= moveDelay)
        {
            moveTimer = 0f;

            UpdateDirection();
            Move();

            if (IsOutOfBounds())
            {
                GameOver("Hit wall");
                return;
            }

            if (HitSelf())
            {
                GameOver("Hit self");
                return;
            }

            CheckFood();
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        inputDirection = ctx.ReadValue<Vector2>();
    }

    private void UpdateDirection()
    {
        Vector2 newDirection = moveDirection;

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            if (inputDirection.x > 0)
            {
                newDirection = Vector2.right;
            }
            else if (inputDirection.x < 0)
            {
                newDirection = Vector2.left;
            }
        }
        else
        {
            if (inputDirection.y > 0)
            {
                newDirection = Vector2.up;
            }
            else if (inputDirection.y < 0)
            {
                newDirection = Vector2.down;
            }
        }

        if (newDirection + moveDirection != Vector2.zero)
        {
            moveDirection = newDirection;
        }
    }

    private void Move()
    {
        positions.Insert(0, transform.position);

        Vector3 move = new Vector3(moveDirection.x, moveDirection.y, 0f);
        transform.position += move * gridSize;

        for (int i = 0; i < bodySegments.Count; i++)
        {
            bodySegments[i].position = positions[i];
        }

        if (positions.Count > bodySegments.Count)
        {
            positions.RemoveAt(positions.Count - 1);
        }
    }

    private bool IsOutOfBounds()
    {
        Vector3 position = transform.position;

        float minX = gridMin.x * gridSize;
        float maxX = gridMax.x * gridSize;
        float minY = gridMin.y * gridSize;
        float maxY = gridMax.y * gridSize;

        return position.x < minX || position.x > maxX || position.y < minY || position.y > maxY;
    }

    private bool HitSelf()
    {
        for (int i = 0; i < bodySegments.Count; i++)
        {
            if (bodySegments[i].position == transform.position)
            {
                return true;
            }
        }

        return false;
    }

    private void Grow()
    {
        Vector3 spawnPosition;

        if (bodySegments.Count == 0)
        {
            spawnPosition = transform.position;
        }
        else
        {
            spawnPosition = bodySegments[bodySegments.Count - 1].position;
        }

        GameObject segment = Instantiate(bodyPrefab, spawnPosition, Quaternion.identity);
        bodySegments.Add(segment.transform);
    }

    private void SpawnFood()
    {
        Vector3 spawnPosition = GetRandomEmptyGridPosition();

        if (foodTransform == null)
        {
            GameObject foodObject = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            foodTransform = foodObject.transform;
        }
        else
        {
            foodTransform.position = spawnPosition;
        }
    }

    private void CheckFood()
    {
        if (foodTransform == null)
        {
            return;
        }

        if (transform.position == foodTransform.position)
        {
            if (gamePlayUIController != null)
            {
                score += 10;
                gamePlayUIController.SetScore(score);
            }

            moveDelay = Mathf.Max(minMoveDelay, moveDelay - speedUpAmount);

            Grow();
            SpawnFood();
        }
    }

    private Vector3 GetRandomEmptyGridPosition()
    {
        while (true)
        {
            int randomX = Random.Range(gridMin.x, gridMax.x + 1);
            int randomY = Random.Range(gridMin.y, gridMax.y + 1);

            Vector3 candidate = new Vector3(randomX * gridSize, randomY * gridSize, 0f);

            if (PositionOccupiedBySnake(candidate) == false)
            {
                return candidate;
            }
        }
    }

    private bool PositionOccupiedBySnake(Vector3 position)
    {
        if (transform.position == position)
        {
            return true;
        }

        for (int i = 0; i < bodySegments.Count; i++)
        {
            if (bodySegments[i].position == position)
            {
                return true;
            }
        }

        return false;
    }

   private void GameOver(string reason)
    {
        isGameOver = true;
        Debug.Log("Game Over: " + reason + " | Final Score: " + score);

        if (gamePlayUIController != null)
        {
            gamePlayUIController.ShowGameOver(true, score);
        }
    }
}
