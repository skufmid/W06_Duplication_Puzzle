using System;
using UnityEngine;

public class ToySoldierButton : MonoBehaviour
{
    public GameObject[] ToySoldiersArray;
    public RuntimeAnimatorController[] Controllers;
    public Sprite[] Sprites; // 0, 1번 인덱스로 Sprite 지정

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

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

    private void ChangeToySoldierId()
    {
        Debug.Log("ChangeToySoldierId");

        int newToySoldierId = -1;

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
                newToySoldierId = 1 - Toy.ToySoldierId;
                Toy.ToySoldierId = newToySoldierId;

                Animator anim = Toy.GetComponentInChildren<Animator>();
                if (anim != null)
                    anim.runtimeAnimatorController = Controllers[newToySoldierId];
            }
        }

        // 자기 자신의 자식 SpriteRenderer에 Sprite 교체
        if (spriteRenderer != null && newToySoldierId >= 0 && Sprites.Length > newToySoldierId)
        {
            spriteRenderer.sprite = Sprites[newToySoldierId];
        }
    }
}