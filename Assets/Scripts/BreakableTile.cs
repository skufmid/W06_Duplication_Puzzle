using UnityEngine;

public class BreakableTile : MonoBehaviour
{
    public GameObject DeathGround;

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Break");
        if (collision.CompareTag("Player"))
        {
            var deathGround = Instantiate(DeathGround, transform.position, Quaternion.identity);
            deathGround.transform.localScale = transform.localScale;
            Destroy(this.gameObject);
        }
    }
}
