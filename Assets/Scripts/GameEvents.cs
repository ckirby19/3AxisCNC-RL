using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	public static GameEvents current;

	private void Awake()
	{
		current = this;
	}

	public event Action OnCubeContactWithGoal;
	public void TriggerCubeContactWithGoal()
	{
		if (OnCubeContactWithGoal != null)
		{
			Debug.Log("Cube contact with goal");
			OnCubeContactWithGoal();
		}
	}
}