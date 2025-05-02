using UnityEngine;
using UnityStandardAssets.Utility;

public class WaypointFollow : MonoBehaviour
{
    public WaypointProgressTracker progressTracker;  // Assign in Inspector or via GetComponent
    public float speed = 10f;
    public float rotSpeed = 5f;
    
    public float acceleration = 20f;
    public float deceleration = 10f;
    private float currentSpeed = 0f;

    public float maxSteerAngle = 30f;  // Maximum steering angle (turning sharpness)
    private float currentSteerAngle = 0f;
    
    // Smooth Steering Curve
    public float smoothSteerFactor = 0.5f;  // Controls how smooth steering is at higher speeds

    void Update()
    {
        if (progressTracker == null || progressTracker.target == null) return;

        // Get the target position from the progress tracker
        Vector3 targetPos = progressTracker.target.position;

        // Direction to the target
        Vector3 direction = targetPos - transform.position;
        direction.y = 0; // Keep car level on Y

        // Calculate the steering angle based on the direction to the target
        float distanceToTarget = direction.magnitude;
        float targetAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up); // Get the angle to the target

        // Smooth steering based on the distance to the target
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, Mathf.Clamp(targetAngle, -maxSteerAngle, maxSteerAngle), Time.deltaTime * rotSpeed);

        // Apply a smooth steering factor for higher speeds (car handles smoother at higher speeds)
        float smoothSteer = Mathf.Lerp(0f, currentSteerAngle, Mathf.Clamp(currentSpeed / speed, 0.2f, 1f));
        
        // Rotate smoothly toward the target with adjusted steering
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y + smoothSteer, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }

        // Accelerate or decelerate smoothly
        if (distanceToTarget > 0.1f) // Only accelerate when there's a direction to go
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Add drift factor at high speeds to simulate sliding around corners
        float driftFactor = Mathf.Clamp(currentSpeed / speed, 0.2f, 1f);
        currentSteerAngle *= driftFactor; // The higher the speed, the more drift

        // Move forward with acceleration
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
