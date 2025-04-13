using UnityEngine;

public class ToySoldier : MonoBehaviour
{
    public int ToySoldierId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>()?.PlayerId != ToySoldierId)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
