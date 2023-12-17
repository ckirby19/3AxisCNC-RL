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
			OnCubeContactWithGoal();
		}
	}
}