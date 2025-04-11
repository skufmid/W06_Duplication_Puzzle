using Unity.VisualScripting;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isEntered;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isEntered = true;
            GameManager.Instance.CheckClear();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isEntered = false;
        }
    }
}
