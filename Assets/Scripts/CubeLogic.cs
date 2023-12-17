using System;
using UnityEngine;

public class CubeLogic : MonoBehaviour
{
    [HideInInspector]
    public bool isInContact;
    [HideInInspector]
    public Collider cubeCollider;

    private float yMotionStageX;
    private float yMotionStageY;
    private float yMotionStageZ;
    private float yMotionStageLengthX = 0.3f;
    private float yMotionStageLengthY = 0.18f;
    private float yMotionStageLengthZ = 0.015f;
    private float cubeLengthX = 0.025f;
    private float cubeLengthY = 0.025f;
    private float cubeLengthZ = 0.025f;
    private GameObject yMotionStage;

	private void Start()
	{
        isInContact = false;
        yMotionStage = GameObject.Find("YMotion");
		yMotionStageX = yMotionStage.transform.position.x;
		yMotionStageY = yMotionStage.transform.position.y;
        yMotionStageZ = yMotionStage.transform.position.z;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Goal")
        {
            GameEvents.current.TriggerCubeContactWithGoal();
            ResetCube();
        }
	}

    // Need to fix cube reset procedure in order to use in training
	private void ResetCube()
    {
        Debug.Log("Cube hit target");
		// Set the XY position to random position across the stage
		isInContact = false;

		yMotionStageX = yMotionStage.transform.localPosition.x;
		yMotionStageY = yMotionStage.transform.localPosition.y;
		yMotionStageZ = yMotionStage.transform.localPosition.z;

		var adjustPosition = new Vector3(
            UnityEngine.Random.Range(-yMotionStageLengthX/2 + cubeLengthX, yMotionStageLengthX/2 - cubeLengthX),
			cubeLengthZ / 2 + 0.0005f,
			UnityEngine.Random.Range(-yMotionStageLengthY/2 + cubeLengthY, yMotionStageLengthY/2 - cubeLengthY)
            );

        var yMotionStageCentre = new Vector3(
            yMotionStageX,
            yMotionStageY,
            yMotionStageZ
            );

        this.transform.position = yMotionStageCentre + adjustPosition; 
    }
}
