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
    public Text missionFailedText;
    public Text missionSuccesfullText;

    public GameObject speechBubble;
    public float speechBubbleDuration = 3f;
    public List<Text> startTextList;
    public List<Text> deathTextList;

    private float endTime;


    public static GameManager GetInstance()
    {
        if (instance) return instance;
        return new GameObject().AddComponent<GameManager>();
    }

    public void HideSpeechBubble()
    {
        speechBubble.active = false;
        foreach (var text in startTextList)
        {
            text.enabled = false;
        }
        foreach (var text in deathTextList)
        {
            text.enabled = false;
        }
    }

    public void ShowRandomSpeechBubble(List<Text> texts)
    {
        HideSpeechBubble();
        speechBubble.active = true;
        var index = UnityEngine.Random.Range (1,(texts.Count - 1));
        texts[index].enabled = true;
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
	        OnPlayerReset(); // Show speech bubble
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

    public void OnPlayerReset()
    {
        HideSpeechBubble();
        ShowRandomSpeechBubble(startTextList);
        Invoke("HideSpeechBubble", speechBubbleDuration);
    }

    public void ReducePlayerLives()
    {
        ShowRandomSpeechBubble(deathTextList);
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
        if (missionFailedText && missionSuccesfullText)
        {
            missionFailedText.enabled = true;
            missionSuccesfullText.enabled = false;
        }
        DisplayEndScreen();
    }

    void CompleteMission()
    {
        Debug.Log("Mission complete!");
        if (missionFailedText && missionSuccesfullText)
        {
            missionFailedText.enabled = false;
            missionSuccesfullText.enabled = true;
        }
            
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
