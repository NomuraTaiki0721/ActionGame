﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class CallStage1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            SceneManager.LoadScene("GameScene");
        }
	}
}
