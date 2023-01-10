using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public int Speed;
    public int StrengthFoward;
    public int StrengthUp;
    public float MouseSense;
    public Transform Orientation;
    public Transform Cam;
    public Transform ObjectHolder;
    public GameObject Projectile;
    private Rigidbody ProjectileBody;
    private Rigidbody rigidBody;
    private bool inHand;
    private PlayerInputActions playerInputActions;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        rigidBody = transform.GetComponent<Rigidbody>();
        ProjectileBody = Projectile.GetComponent<Rigidbody>();
    }
    void Update()
    {
        Move();
        Rotation();
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        inHand = ObjectHolder.Find("Axe");
        Debug.DrawRay(transform.position, forward, Color.red);
    }
    void Move()
    {
        Vector2 moveVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        rigidBody.velocity = transform.TransformDirection(new Vector3(moveVector.x, rigidBody.velocity.y, moveVector.y) * Speed);
    }
    void Rotation()
    {
        Vector2 camVector = playerInputActions.Player.Rotate.ReadValue<Vector2>();
        transform.Rotate(new Vector3(0, camVector.x, 0) * MouseSense);
    }

    public void ThrowObject()
    {
        if (inHand)
        {
            ProjectileBody.velocity = Vector3.zero;
            Vector3 forceDirection = Cam.forward;
            RaycastHit hit;
            
            if(Physics.Raycast(Cam.position, Cam.forward, out hit, 1000f))
            {
                forceDirection = (hit.point - Orientation.position).normalized;
            }

            Vector3 forceToAdd = forceDirection * StrengthFoward + transform.up * StrengthUp;
            ProjectileBody.constraints = RigidbodyConstraints.None;
            ProjectileBody.AddForce(forceToAdd, ForceMode.Impulse);
            Projectile.transform.parent = null;
        }
    }

    public void ReturnObject()
    {
        if (inHand is false)
        {
            Projectile = GameObject.FindGameObjectWithTag("Flying Object");
            Projectile.transform.parent = ObjectHolder;
            Projectile.transform.position = ObjectHolder.position;
            ProjectileBody.velocity = Vector3.zero;
            ProjectileBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
