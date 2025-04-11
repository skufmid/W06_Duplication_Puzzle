using UnityEngine;

public class RainbowZone : MonoBehaviour
{
    public GameObject[] Players;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int changedPlayerId = collision.GetComponent<PlayerController>().PlayerId + 1;
            if (changedPlayerId == 3) changedPlayerId = 0;

            Instantiate(Players[changedPlayerId], collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);

            Destroy(this.gameObject);
        }
    }
}
