using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	public TextMeshProUGUI CountText;
	[SerializeField]
	public GameObject WinText;
	[SerializeField]
	public PlayerController PlayerController;
	[SerializeField]
	public GameObject PowerUpPrefab;
	[SerializeField]
	public int StartingPowerUps;

	private int powerUpsToWin;
	private int powerUpCount;
	private float radius = 7;

	void Start()
	{
		GenerateStartingPowerUps();
		PlayerController.PowerUpCollision += PlayerController_PowerUpCollision; //subscribe to PowerUpCollision event
		powerUpCount = 0;
		powerUpsToWin = (int)(StartingPowerUps * 1.25);
		SetCountText();
		WinText.SetActive(false);
	}

	private void PlayerController_PowerUpCollision(object sender, Collider powerUpCollider)
	{
		powerUpCollider.gameObject.SetActive(false);
		powerUpCount++;
		SetCountText();

		if (powerUpCount == StartingPowerUps)
		{
			StartCoroutine(GenerateTimedPowerUp());
		}
		else if (powerUpCount == powerUpsToWin)
		{
			WinText.SetActive(true);
		}
	}

	private void SetCountText()
	{
		CountText.text = $"Count: {powerUpCount}";
	}

	private void GenerateStartingPowerUps()
	{
		for (int i = 0; i < StartingPowerUps; i++)
		{
			GeneratePowerUpAlongCircle(i);
		}
	}

	private IEnumerator GenerateTimedPowerUp()
	{
		yield return new WaitForSeconds(3);

		var powerUp = GeneratePowerUpAlongCircle(Random.Range(0, StartingPowerUps));

		yield return new WaitForSeconds(3);

		powerUp.SetActive(false);

		if (powerUpCount != powerUpsToWin)
		{
			StartCoroutine(GenerateTimedPowerUp());
		}
	}
	
	private GameObject GeneratePowerUpAlongCircle(int index)
	{
		float theta = (index * 2 * Mathf.PI) / StartingPowerUps;
		float x = Mathf.Cos(theta) * radius;
		float z = Mathf.Sin(theta) * radius;
		return Instantiate(PowerUpPrefab, new Vector3(x, 1, z), Quaternion.identity);
	}
}
