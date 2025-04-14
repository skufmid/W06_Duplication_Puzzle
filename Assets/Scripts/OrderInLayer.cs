using UnityEngine;
using UnityEngine.UIElements;

public class OrderInLayer : MonoBehaviour
{
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        SetOrderInLayer();
    }

    private void SetOrderInLayer()
    {
        _renderer.sortingOrder = 30 - 2 * (int)transform.position.y - 1;
    }
}
