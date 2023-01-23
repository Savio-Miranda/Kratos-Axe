using System;
using UnityEngine;

public class AxeTrajectory : MonoBehaviour {
    [Header("REFERENCES")]
    public GameObject target;
    private Rigidbody targetRigidBody;
    private Transform objectHolder;
    private Rigidbody rigidBody;
    private bool outOfHand = true;

    [Header("MOVEMENT")]
    public float rotateSpeed = 95;

    [Header("PREDICTION")]
    public float _maxDistancePredict = 100;
    public float _minDistancePredict = 5;
    public float _maxTimePrediction = 5;
    private Vector3 _standardPrediction, _deviatedPrediction;

    [Header("DEVIATION")]
    public float _deviationAmount = 50;
    public float _deviationSpeed = 2;

    void Awake(){
        rigidBody = gameObject.GetComponent<Rigidbody>();
        targetRigidBody = target.GetComponent<Rigidbody>();
        objectHolder = target.transform.Find("ObjectHolder");
    }
    void FixedUpdate() {
        rigidBody.velocity = transform.forward * Vector3.Distance(transform.position, target.transform.position);
        var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, target.transform.position));

        if (outOfHand)
        {
            PredictMovement(leadTimePercentage);
            AddDeviation(leadTimePercentage);
            RotateRocket();
        }
    }

    void PredictMovement(float leadTimePercentage) {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = target.transform.position + targetRigidBody.velocity * predictionTime;
    }

    void AddDeviation(float leadTimePercentage) {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;
        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    void RotateRocket() {
        var heading = _deviatedPrediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == target)
        {
            print("collision with target");
            transform.parent = objectHolder;
            transform.position = objectHolder.position;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            outOfHand = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
}