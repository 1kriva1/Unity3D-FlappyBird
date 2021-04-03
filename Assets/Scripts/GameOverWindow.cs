using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();

        transform.Find("retryBtn").GetComponent<Button>().onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        transform.Find("retryBtn").GetComponent<Button>().AddButtonSound();

        transform.Find("menuBtn").GetComponent<Button>().onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenu);
        });
        transform.Find("menuBtn").GetComponent<Button>().AddButtonSound();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Loader.Load(Loader.Scene.GameScene);
        }
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        int score = Level.GetInstance().GetPipesPassedCount();
        int highscore = Score.GetHighscore();
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();

        if (score > highscore)
        {
            highscoreText.text = $"NEW HIGHSCORE: {score}";
        }
        else
        {
            highscoreText.text = $"HIGHSCORE: {Score.GetHighscore()}";
        }
        
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
