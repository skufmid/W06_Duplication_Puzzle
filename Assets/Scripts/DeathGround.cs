using UnityEngine;

public class DeathGround : MonoBehaviour
{
    SpriteRenderer _renderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        int randomIndex = Random.Range(0, 3);
        _renderer.sprite = GameManager.Instance.RoseVines[randomIndex];
        randomIndex = Random.Range(0, 2);
        _renderer.flipX = randomIndex == 1 ? true : false;
    }
}
