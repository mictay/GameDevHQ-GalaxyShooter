﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.N))
        {
            LoadGame();
        }

    }
}