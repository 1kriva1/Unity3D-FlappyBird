using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
    }

    private void Start()
    {
        highscoreText.text = $"HIGHSCORE: {Score.GetHighscore()}";
        Bird.GetInstance().OnDied += ScoreWindow_OnDied;
        Bird.GetInstance().OnStartedPlaying += ScoreWindow_OnStartedPlaying;
    }

    private void ScoreWindow_OnStartedPlaying(object sender, EventArgs e)
    {
        Show();
    }

    private void ScoreWindow_OnDied(object sender, EventArgs e)
    {
        Hide();
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
        highscoreText.text = $"HIGHSCORE: {Score.GetHighscore()}";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
