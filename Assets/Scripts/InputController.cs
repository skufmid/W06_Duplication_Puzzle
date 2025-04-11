using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputController : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();

            // 대각선 제거
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                input = new Vector2(Mathf.Sign(input.x), 0);
            else
                input = new Vector2(0, Mathf.Sign(input.y));

            OnMoveInput?.Invoke(input);
        }
    }
}