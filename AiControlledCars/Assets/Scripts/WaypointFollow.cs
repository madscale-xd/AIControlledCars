using UnityEngine;
using UnityStandardAssets.Utility;

public class WaypointFollow : MonoBehaviour
{
    public WaypointProgressTracker progressTracker;
    public float speed = 20f;
    public float rotSpeed = 8f;

    public float acceleration = 40f;
    public float deceleration = 20f;
    private float currentSpeed = 0f;

    public float maxSteerAngle = 45f;
    private float currentSteerAngle = 0f;

    public float laneWidth = 3f;
    public float avoidRadius = 5f;
    public float avoidStrength = 2.5f; // 🔧 More avoidance push

    public float overtakeOffsetAmount = 4f; // 🔧 Wider lateral moves during overtaking
    public float detectionForwardAngle = 50f; // 🔧 Tighter cone = more aggressive overtakes
    public float boostMultiplier = 1.3f;

    private Vector3 currentTargetOffset;
    private Transform lastTarget;

    void Update()
    {
        if (progressTracker == null || progressTracker.target == null) return;

        if (progressTracker.target != lastTarget)
        {
            currentTargetOffset = GetRandomLaneOffset();
            lastTarget = progressTracker.target;
        }

        Vector3 trueTarget = progressTracker.target.position;
        Vector3 targetPos = trueTarget + currentTargetOffset;

        Vector3 avoidanceOffset = GetAvoidanceOffset();
        targetPos += avoidanceOffset * avoidStrength;

        Vector3 direction = targetPos - transform.position;
        direction.y = 0;

        float distanceToTrueTarget = Vector3.Distance(transform.position, trueTarget);
        float targetAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        currentSteerAngle = Mathf.Lerp(currentSteerAngle, Mathf.Clamp(targetAngle, -maxSteerAngle, maxSteerAngle), Time.deltaTime * rotSpeed);
        float steerFactor = Mathf.Clamp01(currentSpeed / speed);
        float smoothSteer = Mathf.Lerp(0f, currentSteerAngle, steerFactor);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.Euler(0f, transform.eulerAngles.y + smoothSteer, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
        }

        float speedTarget = speed;
        if (IsBlockedAhead(out Vector3 lateralDodge))
        {
            speedTarget *= 0.9f; // 🔧 Don't slow down as much
            targetPos += lateralDodge * (overtakeOffsetAmount + Random.Range(0.5f, 1.5f)); // 🔧 More lateral randomness
        }
        else
        {
            speedTarget *= boostMultiplier;
        }

        currentSpeed = distanceToTrueTarget > 1f
            ? Mathf.MoveTowards(currentSpeed, speedTarget, acceleration * Time.deltaTime)
            : Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    Vector3 GetRandomLaneOffset()
    {
        float offsetX = Random.Range(-laneWidth, laneWidth);
        float offsetZ = Random.Range(-laneWidth, laneWidth);
        return new Vector3(offsetX, 0f, offsetZ);
    }

    Vector3 GetAvoidanceOffset()
    {
        Vector3 avoidance = Vector3.zero;
        Collider[] hits = Physics.OverlapSphere(transform.position, avoidRadius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject != this.gameObject && hit.CompareTag("Car"))
            {
                Vector3 away = transform.position - hit.transform.position;
                away.y = 0;
                float distance = away.magnitude;
                if (distance > 0)
                    avoidance += away.normalized / distance;
            }
        }

        return avoidance.normalized;
    }

    bool IsBlockedAhead(out Vector3 lateralOffset)
    {
        lateralOffset = Vector3.zero;
        Collider[] hits = Physics.OverlapSphere(transform.position, avoidRadius);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == this.gameObject || !hit.CompareTag("Car")) continue;

            Vector3 toOther = hit.transform.position - transform.position;
            toOther.y = 0;

            float angle = Vector3.Angle(transform.forward, toOther);
            if (angle < detectionForwardAngle)
            {
                Vector3 cross = Vector3.Cross(transform.forward, toOther);
                lateralOffset = cross.y > 0 ? -transform.right : transform.right;
                return true;
            }
        }
        return false;
    }
}
