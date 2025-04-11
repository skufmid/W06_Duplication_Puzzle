using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int PlayerId;
    public int Direction;

    public float moveDistance = 1f;
    public float moveSpeed = 5f;

    private bool isMoving = false;
    private Vector3 targetPos;

    private void Start()
    {
        targetPos = transform.position;
    }

    void OnEnable()
    {
        InputController.OnMoveInput += HandleMoveInput;
    }

    void OnDisable()
    {
        InputController.OnMoveInput -= HandleMoveInput;
    }

    void HandleMoveInput(Vector2 dir)
    {
        if (!isMoving)
            StartCoroutine(Move(dir));
    }

    IEnumerator Move(Vector2 dir)
    {
        isMoving = true;
        targetPos = transform.position + (Vector3)(dir * moveDistance * Direction);

        while ((targetPos - transform.position).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
