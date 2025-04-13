using UnityEngine;

public class ToySoldierButton : MonoBehaviour
{
    public GameObject[] ToySoldiersArray;
    public RuntimeAnimatorController[] Controllers;

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

        foreach (GameObject ToySoldiers in ToySoldiersArray)
        {
            if (ToySoldiers == null)
            {
                Debug.LogWarning("Target ToySoldiers not assigned.");
                return;
            }

            ToySoldier[] ToySoldierArray = ToySoldiers.GetComponentsInChildren<ToySoldier>();
            
            foreach (ToySoldier Toy in ToySoldierArray)
            {
                int newToySoldierId = 1 - Toy.ToySoldierId;
                Toy.ToySoldierId = newToySoldierId;

                Animator anim = Toy.GetComponentInChildren<Animator>();
                if (anim != null)
                    anim.runtimeAnimatorController = Controllers[newToySoldierId];
                else
                    Debug.LogWarning("Target ToySoldier has no SpriteRenderer.");
            }
        }
    }
}
