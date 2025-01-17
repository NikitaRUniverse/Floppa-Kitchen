using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floor : MonoBehaviour
{
    public Sprite shadow;
    private bool start = false;
    public PlayerController playerController;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = shadow;
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(DisableSpriteAfterDelay(3.0f));
    }

    public void unlocking()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Starter()
    {
        start = true;
    }

    private IEnumerator DisableSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!start)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController.isWon || playerController.isLost)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
