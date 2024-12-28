using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Image healthBarImage;

	[SerializeField] private TextMeshProUGUI shieldText;
	[SerializeField] private Slider shieldSlider;

	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private Slider levelSlider;

	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI timeText;

	[SerializeField] private TextMeshProUGUI blinkText;
	[SerializeField] private Image blinkProgressBarImage; // Assign your circular Image here

	[SerializeField] private TextMeshProUGUI ultiText;
	[SerializeField] private Image ultimateProgressBarImage; // Assign your circular Image here
	[Range(0f, 1f)] public float progress = 0f; // Progress value (0 = empty, 1 = full)

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
		Color green = new Color(0.44f, 0.79f, 0.22f, 0.8f);

		healthBarImage.color = Color.Lerp(Color.red, green, healthSlider.value / healthSlider.maxValue);

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
			blinkText.text = "tele".ToString();
		} else
        {
			blinkText.text = score.ToString("F2");
		}

		blinkProgressBarImage.fillAmount = 1 - (score / 5);

	}


	public void UpdateUltiValue(float score)
	{
		ultimateProgressBarImage.fillAmount = score / 100;
		ultiText.text = (Mathf.Ceil(score)).ToString() + "%";
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
