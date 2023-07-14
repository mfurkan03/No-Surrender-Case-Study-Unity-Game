using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScript : MonoBehaviour
{
    // Serialized and private, only is exposed to Unity
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float accelerationConstant = 4f;
    [SerializeField] private float frictionAccelerationConstant = 1f;
    [SerializeField] private float maxSpeed = 9f;
    [SerializeField] private LayerMask torusLayer;
    [SerializeField] private LayerMask fighterLayer;
    [SerializeField] private HitRules hitRules;
    [SerializeField] private GameObject playersTransforms;

    private float bounceStickConstant;
    private float hitBounceConstant;
    private float hitAnimationDuration;
    private float scoreMultiplier;
    //checks if the player is colliding for the first time
    //private bool firstTouch = true;

    //private bool canMove = true;
    private Vector3 frictionAcceleration;
    private Vector3 acceleration;
    private Vector3 moveVelocity = Vector3.zero;
    private Vector3 lastMoveDirection;

    //this is a bool to check if the component owner got hit first or second, to optimize hitting another player, with this,
    //we are able to trade the velocities 
    
    // for creating only one coroutine at a time
    private bool isLerping = false;
    private bool isWalking ;
    private bool isPushing ;
    private bool isSlowing ;

    //last player who hit us gets point if we die
    private Transform lastHitPlayer;
    private void Start()
    {
        bounceStickConstant = hitRules.bounceStickConstant;
        hitBounceConstant = hitRules.hitBounce;
        hitAnimationDuration = hitRules.hitAnimationDuration;
        scoreMultiplier = hitRules.scoreMultiplier;
    }


    //Update is called once per frame
    private void Update()
    {
        HandleMovement();
        isPushing = false;
    }


    public bool IsWalking()
    { 
        return isWalking;
    }
    public bool IsPushing() {  return isPushing;}
    public bool IsSlowing() {  return isSlowing;}
    //Handling all the movement and animations
    private void HandleMovement()
    {
        //converting 3d movementVector into 3d
        Vector2 moveDir2 = gameInput.GetTouchDragVectorNormalized();
        Vector3 moveDir3 = new Vector3(moveDir2.x,0f,moveDir2.y);

        acceleration = accelerationConstant * moveDir3;
        frictionAcceleration = frictionAccelerationConstant * moveVelocity.normalized;
        //If the player is dragging, add force as acceleration, if the speed is maxed, only friction will be neglected
        if (moveDir3 != Vector3.zero)
        {
            //for walking animation

            isWalking = true;
            isSlowing = false;
            lastMoveDirection = moveDir3;
            if (moveVelocity.magnitude < maxSpeed)
            {
                moveVelocity += acceleration * Time.deltaTime;
            }
            else if (moveVelocity.magnitude > maxSpeed)
            {
                //neglecting friction for pressing button on maximum speed occasions and normalizing velocity to evoid errors
                moveVelocity = moveVelocity.normalized*maxSpeed;
            }
            else
            {//if moveVelocity == maxSpeed
                moveVelocity += frictionAcceleration;
            }
            //rotates the player head to moveing direction
            float rotateSpeed = 2f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir3, rotateSpeed * Time.deltaTime);
        }
        //No matter player is dragging or not, if the character is moving there is a friction force slowing it down
        //If player is at max speed and still dragging, the acceleration will be bigger than friction but won't surpass it,
        //therefore friction and acceleration can be neglected and the movespeed won't be changed
        else{
            //for walking animation
            isWalking = false;
            if (moveVelocity.magnitude > .1f)
            {
                isSlowing = true;
            }
            else { isSlowing = false; }
            if (moveVelocity.magnitude >= maxSpeed)
            {
                moveVelocity = moveVelocity.normalized * maxSpeed;
            }
        }

        moveVelocity -= frictionAcceleration * Time.deltaTime;

        //this code is for moving the player till they stop even they don't drag the screen

        Vector3 moveDisplacement = moveVelocity*Time.deltaTime;
        transform.position += moveDisplacement;

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision occurred with an object on the torus layer
        if (torusLayer == (torusLayer | (1 << collision.gameObject.layer)))
        {
            // Get the collision point
            Vector3 collisionPoint = collision.contacts[0].point;

            // Get the collision normal
            Vector3 collisionNormal = collision.contacts[0].normal;
            //convert collusion into 2d components
            Vector2 collisionNormal2D = new Vector2(collisionNormal.x, collisionNormal.z);

            // Reflect the character's velocity
            Vector2 reflectedVelocity = Vector2.Reflect(new Vector2(moveVelocity.x,moveVelocity.z), collisionNormal2D);

            // Assign the reflected velocity to the character's rigidbody
            moveVelocity = new Vector3(reflectedVelocity.x,0f, reflectedVelocity.y)*bounceStickConstant;
        }
        else if(fighterLayer == (fighterLayer | (1<< collision.gameObject.layer)))
        {
            Hit(collision);

        }
    }

    public void Hit(Collision collision)
    {


        if (collision.collider.gameObject.TryGetComponent<Navmesh>(out Navmesh script))
        {

            Vector3 enemyVelocity = script.GetMovementVelocity();
            if (enemyVelocity.magnitude > moveVelocity.magnitude)
            {

                //I won't add the enemyvelocity.magnitude < movevelocity.magnitude condition because it automatically
                //will be handled in enemy's script
                Vector3 endPosition = transform.position + hitBounceConstant * enemyVelocity.normalized;
                lastHitPlayer = collision.transform;
                if (!isLerping)
                {
                    StartCoroutine(LerpPosition(hitAnimationDuration, transform.position, endPosition));
                }

                
            }
            else if (enemyVelocity.magnitude < moveVelocity.magnitude)
            {
                moveVelocity = moveVelocity / 4;
                //basically if you hit faster than enemy, it adds point proportional to how fast you were
                AddPoints(Mathf.FloorToInt(scoreMultiplier * moveVelocity.magnitude));
                isPushing = true;
            }
            else
            {
                
            }

        }
    }
    IEnumerator LerpPosition(float duration, Vector3 startTransformPosition, Vector3 endTransformPosition)
    {
        isLerping = true;
        float timeElapsed = 0f;
        float t = 0f;
        while (timeElapsed < duration)
        {
            float progress = 0;
            progress = t / duration;
            progress = SmoothProgress(progress);
            transform.position = Vector3.Lerp(startTransformPosition, endTransformPosition, progress);
            timeElapsed += Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

 
        Debug.Log("lerp finished");

        isLerping = false;
    }
    public float SmoothProgress(float progress)
    {
        progress = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, progress);
        progress = Mathf.Sin(progress);
        progress = (progress / 2f) + .5f;
        return progress;
    }

    public void Dying()
    {
        playersTransforms.GetComponent<PlayerStorage>().AddScore(lastHitPlayer, 15);
        
    }


    public Vector3 GetMovementVelocity()
    {
        return moveVelocity;
    }
    public void SetMovementVelocity(Vector3 newVelocity)
    {
       moveVelocity = newVelocity;
    }
    /// <summary>
    /// takes integer point, accesses the player data manager object via serialized Field and adds point to the data
    /// basically earn point function
    /// </summary>
    /// <param name="point"> the amount of point to add</param>
    private void AddPoints(int point)
    {
        playersTransforms.GetComponent<PlayerStorage>().AddScore(transform, point);
    }
}

