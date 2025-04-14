using UnityEngine;

public class ToySoldier : MonoBehaviour
{
    public int ToySoldierId;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        SetOrderInLayer();
    }
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

    private void SetOrderInLayer()
    {
        _renderer.sortingOrder = 30 - 2 * (int)transform.position.y - 1;
    }
}
