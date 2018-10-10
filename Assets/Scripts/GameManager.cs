using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject tutorialScreen;
    public bool tutorialScreenVisible = true;
    
   
    
    public Text timeText;
    public Text scoreText;
    public Text cargoText;
    public Text livesText;
    public Text floatingText;
    public int livesLeft;
    private int ufoDestroyed;
    int playerScore;
    int cargoCollected;
    int cargoTotal;
    DateTime startTime;
    
    public GameObject scoreScreen;
    public bool scoreScreenVisible = false;
    
    public Text endCargoText;
    public Text endTimeText;
    public Text endScoreText;
    public Text endUfoText;
    public Text endMessage;
    public String missionFailedText;
    public String missionSuccesfullText;

    private float endTime;


    public static GameManager GetInstance()
    {
        if (instance) return instance;
        return new GameObject().AddComponent<GameManager>();
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        HideFloatingText();
        tutorialScreen.active = tutorialScreenVisible;
        scoreScreen.active = false;
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
	    if (tutorialScreenVisible && Input.anyKey)
	    {
	        tutorialScreenVisible = false;
	        tutorialScreen.active = false;
	        Debug.Log("Tutorial disabled!");
	    }
	    if (!scoreScreenVisible)
	        UpdateUI();
	    else if (Input.anyKey && Time.time - endTime > 2) // Reload scene on button press, if atleast n seconds has passed
	        Application.LoadLevel(Application.loadedLevel);

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

    public void IncrementUfoCounter()
    {
        ufoDestroyed += 1;
    }

    public void ReducePlayerLives()
    {
        livesLeft -= 1;
        if (livesLeft < 0)
        {
            livesLeft = 0;
            FailMission();
        }
            
    }

    public void CollectCargo()
    {
        cargoCollected += 1;
        if (CountRemainingCargo() == 0)
            CompleteMission();
    }

    void FailMission()
    {
        Debug.Log("Mission failed!");
        if (endMessage)
            endMessage.text = missionFailedText;
        DisplayEndScreen();
    }

    void CompleteMission()
    {
        Debug.Log("Mission complete!");
        if (endMessage) 
            endMessage.text = missionSuccesfullText;
        DisplayEndScreen();
    }

    void DisplayEndScreen()
    {
        scoreScreenVisible = true;
        endTime = Time.time;
        scoreScreen.active = true;
        endScoreText.text = playerScore.ToString();
        endUfoText.text = ufoDestroyed.ToString();
        endCargoText.text = string.Format("{0} / {1}", cargoCollected, cargoTotal);
        var time = DateTime.Now - startTime;
        endTimeText.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
        var player = FindObjectOfType<SpaceShip>();
        if (player)
        {
            player.gameObject.active = false;
        }
    }

    int CountRemainingCargo()
    {
        return FindObjectsOfType<Payload>().Length;
    }

    public void DisplayFloatingText(String text, float duration)
    {
        CancelInvoke("HideFloatingText");
        floatingText.text = text;
        //Invoke("HideFloatingText", duration);
        StartCoroutine(FadeFloatingText(duration));
    }

    void HideFloatingText()
    {
        floatingText.text = "";
    }

    public IEnumerator FadeFloatingText(float t)
    {
        var color = floatingText.color;
        color.a = 1;
        while (color.a > 0.0f)
        {
            color.a -= (Time.deltaTime / t);
            floatingText.color = color;
            yield return null;
        }
    }
}
