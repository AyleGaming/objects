using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class UIManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private Slider healthSlider;

	[SerializeField] private TextMeshProUGUI shieldText;
	[SerializeField] private Slider shieldSlider;

	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private Slider levelSlider;

	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI ultiText;
	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private TextMeshProUGUI blinkText;

	public float elapsedTime = 0f;

	// Setup Listeners for when actions become available
	private void OnEnable()
	{
		Character.OnCharacterInitialized += SetupCharacterListenersForUI;
		ScoreManager.OnScoreManagerInit += SetupScoreManagerListenersForUI;
		GameManager.OnGameManagerInit += SetupGameManagerListenersForUI;
	}

	private void OnDisable()
	{
		Character.OnCharacterInitialized -= SetupCharacterListenersForUI;
		ScoreManager.OnScoreManagerInit -= SetupScoreManagerListenersForUI;
		GameManager.OnGameManagerInit -= SetupGameManagerListenersForUI;
	}

	private void SetupCharacterListenersForUI(Character character)
	{
		if (character is Player player)
		{
			if (player.healthValue != null)
			{
				player.healthValue.OnHealthChanged.AddListener(UpdateHealthValue);
				UpdateHealthValue(player.healthValue.GetHealthValue());

				player.healthValue.OnShieldChanged.AddListener(UpdateShieldValue);
				UpdateShieldValue(player.healthValue.GetShieldValue());

				player.BlinkCoolDownUpdate.AddListener(UpdateBlinkValue);
			}
			else
			{
				Debug.LogError("UIManager: Player's healthValue is not initialized.");
			}
		}
	}

	private void SetupScoreManagerListenersForUI(ScoreManager scoreManager)
	{
		if (scoreManager.OnScoreChanged != null)
		{
			scoreManager.OnScoreChanged.AddListener(UpdateScoreValue);
		}
		else
		{
			Debug.LogError("UIManager: Score manager OnScoreChanged is not initialized.");
		}
	}

	private void SetupGameManagerListenersForUI(GameManager gameManager)
	{
		if (gameManager != null)
		{
			gameManager.OnUltimateStatusChanged.AddListener(UpdateUltiValue);
			gameManager.OnLevelPercentStatusChanged.AddListener(UpdateLevelSliderValue);
			gameManager.OnLevelUpChanged.AddListener(UpdateLevelValue);
		}
		else
		{
			Debug.LogError("UIManager: Score manager OnScoreChanged is not initialized.");
		}

	}


	void Update()
	{
		// Increment elapsed time
		elapsedTime += Time.deltaTime;

		// Format the timer and update the UI
		string formattedTime = FormatTime(elapsedTime);
		UpdateTimeValue(formattedTime);
	}

	public void UpdateScoreValue(int score)
	{
		scoreText.text = score.ToString();
	}

	public void UpdateHealthValue(float score)
	{
		healthText.text = score.ToString();
		healthSlider.value = score;
	}

	public void UpdateShieldValue(float score)
	{
		shieldText.text = score.ToString();
		shieldSlider.value = score;
	}

	public void UpdateBlinkValue(float score)
	{
		if(score <= 0)
        {
			blinkText.text = "READY".ToString();
		} else
        {
			blinkText.text = score.ToString();
		}
	}

	public void UpdateUltiValue(float score)
	{
		if(score >= 100)
        {
			ultiText.text = "READY".ToString();
        } else
        {
			ultiText.text = score.ToString() + "%";
		}
	}

	public void UpdateLevelSliderValue(float score)
	{
		levelSlider.value = score;
	}

	public void UpdateLevelValue(int score)
	{
		levelText.text = score.ToString();
	}


	public void UpdateTimeValue(string score)
	{
		timeText.text = score.ToString();
	}

	// Formats the time as minutes:seconds
	private string FormatTime(float time)
	{
		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);

		if (minutes > 0)
		{
			return string.Format("{0:00}:{1:00}", minutes, seconds); // Show minutes and seconds
		}
		else if (seconds > 0)
		{
			return seconds.ToString();
		}
		else
		{
			return "0"; // If time is zero or less, display "0"
		}
	}
}
