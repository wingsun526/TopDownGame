using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    //[SerializeField] private float pushRecoveryTime = 0.3f;
    [SerializeField] private float pushRecoveryAmount = 0.2f;
    
    
    [Header("Weapon")]
    [SerializeField] private float weaponPushForce = 5.0f;
    [SerializeField] private float weaponReloadTime = 1.0f;
    
    
    private Vector2 moveInput;
    private Animator myAnimator;
    private Rigidbody2D myRigidbody;
    private SpriteRenderer mySpriteRenderer;

    private bool beingPush = false;
    private Vector2 mouseWorldPosition;
    private float lastPush;
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Run();
        FlipSprite();
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnFire(InputValue value)
    {
        PushPlayer();
        
    }
    void PushPlayer()
    {
        if (Time.time - lastPush > weaponReloadTime)
        {
            beingPush = true;
            lastPush = Time.time;
            // shoot the player to the direction with a force
            // difference Forcemode are available
            /*  ForceMode.Force - Applies a gradual force on the Object, taking mass into account.
                    This is a literal pushing motion where the bigger the mass of the Object, the slower it will speed up.
                ForceMode.Impulse - Applies an instant force on the Object, taking mass into account. This pushes the Object 
                    using the entire force in a single frame. Again, the bigger the mass of the Object, the less effect this will have. Great for recoil or jump functions.
                ForceMode.Acceleration - Same as ForceMode.Force except that it doesn't take mass into account. 
                    No matter how big the mass of the object, it will accelerate at a constant rate.
                ForceMode.VelocityChange - Same as ForceMode.Impulse and again, doesn't take mass into account. 
                    It will literally add the force to the Object's velocity in a single frame.*/
            // rigidbody drag will act on this force over time.
            myRigidbody.AddForce(-(mouseWorldPosition - (Vector2)myRigidbody.transform.position).normalized * weaponPushForce, ForceMode2D.Impulse);
        }
    }
    private void Run()
    {
        // player lose control when being pushed by force, gains back control when velocity smaller than a certain amount
        if (Math.Abs(myRigidbody.velocity.x) + Math.Abs(myRigidbody.velocity.y)  < pushRecoveryAmount)//(Time.time - lastPush > pushRecoveryTime)
        {
            beingPush = false;
        }

        if (!beingPush)
        {
            
            Vector2 playerVelocity = new Vector2(moveInput.x, moveInput.y) * speed;
            myRigidbody.velocity = playerVelocity;
        
    
    
            // For starting up the move animation
            bool playerIsMoving = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y) > Mathf.Epsilon;
            myAnimator.SetBool("isMoving", playerIsMoving);
            
        }

        
        
    }
    private void FlipSprite()
    {
        if (mouseWorldPosition.x > myRigidbody.position.x)
        {
            mySpriteRenderer.flipX = false;
        }
        else if (mouseWorldPosition.x < myRigidbody.position.x)
        {
            mySpriteRenderer.flipX = true;
        }
    }

}
