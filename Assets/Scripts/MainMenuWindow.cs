using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("playBtn").GetComponent<Button>().onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        transform.Find("playBtn").GetComponent<Button>().AddButtonSound();

        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(() => {
            Application.Quit();
        });
        transform.Find("quitBtn").GetComponent<Button>().AddButtonSound();
    }
}
