using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameplayScene : MonoBehaviour
{
    public void Pressed()
    {
        SceneManager.LoadScene("Gameplay");
    }
}