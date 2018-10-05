using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Text timeText;
    public Text scoreText;
    public Text cargoText;
    public Text livesText;
    public Text fuelText;
    public int livesLeft;
    int playerScore;
    int cargoCollected;
    int cargoTotal;
    DateTime startTime;


    public static GameManager GetInstance()
    {
        if (instance) return instance;
        return new GameObject().AddComponent<GameManager>();
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        cargoTotal = CountRemainingCargo();
        startTime = DateTime.Now;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    UpdateUI();

	}

    void UpdateUI()
    {
        livesText.text = livesLeft.ToString();
        //fuelText.text = 100.ToString(); // TODO: Implement me
        cargoText.text = string.Format("{0} / {1}", cargoCollected, cargoTotal);
        scoreText.text = playerScore.ToString();
        var time = DateTime.Now - startTime;
        timeText.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }

    public void AddToScore(int score)
    {
        playerScore += score;
    }

    public void ReducePlayerLives()
    {
        livesLeft -= 1;
        if (livesLeft <= 0)
            FailMission();
    }

    public void CollectCargo()
    {
        cargoCollected += 1;
        if (CountRemainingCargo() == 0)
            CompleteMission();
    }

    void FailMission()
    {
        // Implement me
        Debug.Log("Mission failed!");
    }

    void CompleteMission()
    {
        // Implement me
        Debug.Log("Mission complete!");
    }

    int CountRemainingCargo()
    {
        return FindObjectsOfType<Payload>().Length;
    }
}
