using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public GameObject objectToThrow;
    public Transform attackPoint;
    public Transform cam;
    public float SenseX;
    public float Speed;
    public float Strength;
    public bool readyToThrow;
    private Rigidbody rb;
    private PlayerInputActions playerInputActions;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        rb = transform.GetComponent<Rigidbody>();
        readyToThrow = true;
    }

    void Update()
    {
        Move();
        Rotation();
    }
    void Move()
    {
        Vector2 moveVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        rb.velocity = transform.TransformDirection(new Vector3(moveVector.x, rb.velocity.y, moveVector.y) * Speed);
    }

    void Rotation()
    {
        Vector2 camVector = playerInputActions.Player.Rotate.ReadValue<Vector2>();
        transform.Rotate(new Vector3(0, camVector.x, 0) * SenseX * Time.deltaTime);
    }
    
    public void ThrowObject()
    {
        readyToThrow = false;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        Rigidbody projectileRigidBody = projectile.GetComponent<Rigidbody>();
        Vector3 forceToAdd = cam.forward * Strength / 2 + transform.up * Strength / 5;
        projectileRigidBody.AddForce(forceToAdd, ForceMode.Impulse);
    }

    public void GetObjectBack()
    {
        return;
    }
}
