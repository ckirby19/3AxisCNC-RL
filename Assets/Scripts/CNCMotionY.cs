using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CNCMotionY : MonoBehaviour
{
    #region Variables
    [Range(-0.1f,0.1f)]
    public float positionAdjustY = 0;
    [Range(0.01f,1f)] // Will later change this to actual CNC max speeds
    public float speedXY = 1;

    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var targetPosition = new Vector3(0,
                                         0,
                                         positionAdjustY);
        transform.position = Vector3.MoveTowards(transform.position,
                                                 targetPosition,
                                                 Time.fixedDeltaTime * speedXY);

    }
}
