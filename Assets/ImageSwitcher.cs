using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Image imageUI;
    public Sprite defaultSprite;  // Default image to show after GIF ends
    public float displayDuration = 5f;  // Time to loop the GIF before reverting
    public float gifFrameTime = 0.1f;  // Time to show each frame in a GIF

    private Dictionary<string, Sprite[]> gifs = new Dictionary<string, Sprite[]>();
    private Coroutine gifCoroutine;
    private bool isEndlessGifRunning = false;  // Tracks if an endless GIF is currently running

    private void Start()
    {
        LoadGifFrames("Key Found");
        LoadGifFrames("Sword");
        LoadGifFrames("Fire Sword");
        LoadGifFrames("Wall");
        LoadGifFrames("Enemy");

        imageUI.sprite = defaultSprite;
    }

    private void LoadGifFrames(string gifName)
    {
        // Load all frames for a GIF from Resources
        Sprite[] frames = Resources.LoadAll<Sprite>(gifName);
        if (frames.Length > 0)
        {
            gifs[gifName] = frames;
        }
    }

    // Starts playing a GIF, with option for it to be endless
    public void PlayGif(string gifName, bool isEndless = false)
    {
        // If an endless GIF is running, ignore other GIF play requests
        if (isEndlessGifRunning && !isEndless)
        {
            return;
        }

        // Stop any currently running GIF coroutine
        if (gifCoroutine != null)
        {
            StopCoroutine(gifCoroutine);
        }

        isEndlessGifRunning = isEndless;

        gifCoroutine = StartCoroutine(DisplayGifLoop(gifName, isEndless));
    }

    private IEnumerator DisplayGifLoop(string gifName, bool isEndless)
    {
        Sprite[] frames = gifs[gifName];
        float elapsedTime = 0f;

        // Loop GIF indefinitely if `isEndless` is true, or for a specific duration otherwise
        while (isEndless || elapsedTime < displayDuration)
        {
            foreach (Sprite frame in frames)
            {
                imageUI.sprite = frame;
                yield return new WaitForSeconds(gifFrameTime);

                // Update elapsed time after each frame is shown (only for non-endless GIFs)
                if (!isEndless)
                {
                    elapsedTime += gifFrameTime;
                    if (elapsedTime >= displayDuration)
                    {
                        break;
                    }
                }
            }
        }

        // If not endless, revert to default image after display duration
        if (!isEndless)
        {
            imageUI.sprite = defaultSprite;
            isEndlessGifRunning = false;
        }
    }

    // Manually stops an endless GIF and reverts to the default image
    public void StopEndlessGif()
    {
        if (isEndlessGifRunning)
        {
            if (gifCoroutine != null)
            {
                StopCoroutine(gifCoroutine);
                gifCoroutine = null;
            }
            imageUI.sprite = defaultSprite;
            isEndlessGifRunning = false;
        }
    }
}
