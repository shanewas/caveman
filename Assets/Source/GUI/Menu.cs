﻿using Caveman.GUI;
using UnityEngine;

public class Menu : MonoBehaviour 
{
    public void LoadSingleGame()
    {
        print(LoadingScreen.instance == null);
        LoadingScreen.instance.ProgressTo(1);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        Application.LoadLevel(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        Application.LoadLevel(2);
    }
}
