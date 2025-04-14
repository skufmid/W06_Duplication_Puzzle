using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakableTile : MonoBehaviour
{
    public GameObject DeathGround;

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Break");
        if (collision.CompareTag("Player") && gameObject.scene.isLoaded)
        {
            InstantiateDeathGround();
            Destroy(this.gameObject);
        }
    }

    private void InstantiateDeathGround()
    {
        var deathGround = Instantiate(DeathGround, transform.position, Quaternion.identity);
        deathGround.transform.localScale = transform.localScale;
    }
}
