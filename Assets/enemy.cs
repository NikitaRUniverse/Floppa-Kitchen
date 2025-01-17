using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class enemy : MonoBehaviour
{
    public Sprite og;
    public Sprite og2;
    public Sprite shadow;
    public PlayerController playerController;

    public bool is2;
    private static readonly System.Random random = new System.Random();

    void Start()
    {
        is2 = GetRandomBool();
        if (!is2)
        {
            GetComponent<SpriteRenderer>().sprite = og;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = og2;
        }

        StartCoroutine(DisableSpriteAfterDelay(3.0f));
    }

    public void unlocking()
    {
        if (!is2)
        {
            GetComponent<SpriteRenderer>().sprite = og;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = og2;
        }
    }

    public bool Is2()
    {
        return is2;
    }

    private IEnumerator DisableSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<SpriteRenderer>().sprite = shadow;
    }

    public static bool GetRandomBool()
    {
        return random.Next(2) == 1;
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController.isWon || playerController.isLost)
        {
            if (!is2)
            {
                GetComponent<SpriteRenderer>().sprite = og;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = og2;
            }
        }
    }
}
