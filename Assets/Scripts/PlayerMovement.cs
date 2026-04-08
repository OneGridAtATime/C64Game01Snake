using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveDelay = 0.2f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private GameObject bodyPrefab;

    private GameInputActions input;

    private Vector2 inputDirection;
    private Vector2 moveDirection = Vector2.right;

    private float moveTimer;

    private List<Transform> bodySegments = new List<Transform>();
    private List<Vector3> positions = new List<Vector3>();

    private void Awake()
    {
        input = new GameInputActions();
    }

    private void Start()
    {
        // Start with 1 body segment
        Grow();
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
        moveTimer += Time.deltaTime;

        if (moveTimer >= moveDelay)
        {
            moveTimer = 0f;

            UpdateDirection();
            Move();
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        inputDirection = ctx.ReadValue<Vector2>();
    }

    private void UpdateDirection()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space pressed - Grow snake");
            Grow();
        }

        Vector2 newDirection = moveDirection;

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            if (inputDirection.x > 0)
                newDirection = Vector2.right;
            else if (inputDirection.x < 0)
                newDirection = Vector2.left;
        }
        else
        {
            if (inputDirection.y > 0)
                newDirection = Vector2.up;
            else if (inputDirection.y < 0)
                newDirection = Vector2.down;
        }

        // Block reverse direction
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

    private void Grow()
    {
        GameObject segment = Instantiate(bodyPrefab);
        bodySegments.Add(segment.transform);
    }
}
