using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class key : MonoBehaviour
{
    public Sprite og;
    public Sprite shadow;
    public PlayerController playerController;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = og;
        StartCoroutine(DisableSpriteAfterDelay(3.0f));
    }

    public void unlocking()
    {
        GetComponent<SpriteRenderer>().sprite = og;
    }

    private IEnumerator DisableSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<SpriteRenderer>().sprite = shadow;
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController.isLost)
        {
            GetComponent<SpriteRenderer>().sprite = og;
        }
    }
}