using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static int GetHighscore()
    {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetHighscore(int score)
    {
        if (score > GetHighscore())
        {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }
}
