using UnityEngine;

public class ToySoldierButton : MonoBehaviour
{
    public ToySoldier[] ToySoldiers;
    public Sprite[] newSprite;

    private int newToySoldierId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeToySoldierId();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeToySoldierId();
        }
    }

    private void Update()
    {
    }

    private void ChangeToySoldierId()
    {
        Debug.Log("ChangeToySoldierId");

        if (ToySoldiers == null)
        {
            Debug.LogWarning("Target ToySoldier not assigned.");
            return;
        }

        foreach (var Toy in ToySoldiers)
        {
            int newToySoldierId = 1 - Toy.ToySoldierId;
            Toy.ToySoldierId = newToySoldierId;

            SpriteRenderer sr = Toy.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = newSprite[newToySoldierId];
            else
                Debug.LogWarning("Target ToySoldier has no SpriteRenderer.");
        }
    }
}
