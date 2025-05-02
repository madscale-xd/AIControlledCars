using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowGoal : MonoBehaviour {

	public Transform goal;
	float brakeAngle = 60.0f;

    private CarSystem movementSystem;

    private void Start()
    {
        movementSystem = GetComponent<CarSystem>();
    }

    private void LateUpdate()
    {
        Vector3 lookAtGoal = new Vector3(goal.position.x, transform.position.y, goal.position.z);
        Vector3 direction = lookAtGoal - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), movementSystem.rotSpeed * Time.deltaTime);

        float angle = Vector3.Angle(goal.forward, transform.forward);
        if (angle > brakeAngle)
        {
            movementSystem.ApplyDeceleration();
        }
        else
        {
            movementSystem.ApplyAcceleration();
        }

        movementSystem.ApplyTranslation();
    }
}
