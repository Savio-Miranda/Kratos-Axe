using System;
using UnityEngine;

public class AxeTrajectory : MonoBehaviour {
    [Header("REFERENCES")]
    public GameObject target;
    private Rigidbody targetRb;
    private Transform objectHolder;
    private Rigidbody rb;

    [Header("MOVEMENT")]
    public float rotateSpeed = 95;

    [Header("PREDICTION")]
    public float _maxDistancePredict;
    public float _minDistancePredict;
    public float _maxTimePrediction;
    private Vector3 _standardPrediction, _deviatedPrediction;

    [Header("DEVIATION")]
    public float _deviationAmount = 50;
    public float _deviationSpeed = 2;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        targetRb = target.GetComponent<Rigidbody>();
        objectHolder = target.transform.Find("ObjectHolder");
    }

    void FixedUpdate() {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            Start();
        
        rb.velocity = transform.forward * Vector3.Distance(transform.position, target.transform.position);
        var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, target.transform.position));
        
        if (gameObject.GetComponent<AxeTrajectory>().enabled is false)
        {
            return;
        }

        PredictMovement(leadTimePercentage);
        AddDeviation(leadTimePercentage);
        RotateRocket();
    }

    void PredictMovement(float leadTimePercentage) {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = target.transform.position + targetRb.velocity * predictionTime;
    }

    void AddDeviation(float leadTimePercentage) {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;
        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    void RotateRocket() {
        var heading = _deviatedPrediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
    }

    void OnCollisionEnter(Collision collision) {
        bool condition = gameObject.GetComponent<AxeTrajectory>().isActiveAndEnabled;
        if (collision.gameObject == target && condition)
        {
            transform.parent = objectHolder;
            transform.localPosition = objectHolder.localPosition;
            Destroy(rb);
            gameObject.GetComponent<AxeTrajectory>().enabled = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
}