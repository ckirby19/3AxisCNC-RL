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
	public event Action OnCubeContactWithEndEffector;
	public void TriggerCubeContactWithGoal()
	{
		if (OnCubeContactWithGoal != null)
		{
			OnCubeContactWithGoal();
		}
	}
	public void TriggerCubeContactWithEndEffector()
	{
		if (OnCubeContactWithEndEffector != null)
		{
			OnCubeContactWithEndEffector();
		}
	}
}