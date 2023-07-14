using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Navmesh : MonoBehaviour
{

    private NavMeshAgent agent;


    [SerializeField] private LayerMask torusLayer;
    [SerializeField] private LayerMask fighterLayer;
    [SerializeField] private HitRules hitRules;
    [SerializeField] private GameObject playersTransforms;
    private float bounceStickConstant ;
    private float hitBounceConstant ;
    private float hitAnimationDuration;
    private float scoreMultiplier;
    private Vector3 moveVelocity = Vector3.zero;

    //last one who hit us gets point if we die(counts as kill)
    private Transform lastHitPlayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        bounceStickConstant = hitRules.bounceStickConstant;
        hitBounceConstant = hitRules.hitBounce;
        hitAnimationDuration = hitRules.hitAnimationDuration;
        scoreMultiplier = hitRules.scoreMultiplier;
    }

    private void FixedUpdate()
    {
        if (agent.enabled)
        {
            isPushing = false;
            agent.destination = GetNearestPlayerTransform().position;
            float rotateSpeed = .7f;
            agent.transform.forward = Vector3.Slerp(agent.transform.forward, agent.desiredVelocity.normalized, rotateSpeed );

            if (agent.destination!=Vector3.zero) {
                isWalking = true;
                isSlowing = false;
            }
            else
            {
                isWalking = false;
                if (agent.velocity.magnitude > 0) {
                    isSlowing = true;
                }
            }
        }
    }
    ////////////////animation section
    private bool isWalking;
    private bool isPushing;
    private bool isSlowing; 
    public bool IsWalking()
    {
        return isWalking;
    }
    public bool IsPushing() { return isPushing; }
    public bool IsSlowing() { return isSlowing; }
    //////////////////////////////////////
    private Transform GetNearestPlayerTransform()
    {
        float nearestDistance = float.MaxValue;
        Transform nearestPlayerTransform = null;
        foreach (Transform playerTransform in playersTransforms.GetComponent<PlayerStorage>().GetPlayersTransforms())
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            //distance should be greater than .1f else the ai willl go to itself as nearest
            if (distance < nearestDistance&&distance>.1f)
            {
                nearestPlayerTransform = playerTransform;
                nearestDistance = distance;
            }
        }

        return nearestPlayerTransform;
    }
    /// <summary>
    /// if collision is occured with a bounce stick, the velocity of object will be reflected, else
    /// hit operation will occur
    /// </summary>
    /// <param name="collision"></param>
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
            Vector2 reflectedVelocity = Vector2.Reflect(new Vector2(moveVelocity.x, moveVelocity.z), collisionNormal2D);

            // Assign the reflected velocity to the character's rigidbody
            moveVelocity = new Vector3(reflectedVelocity.x, 0f, reflectedVelocity.y) * bounceStickConstant;
        }
        else if (fighterLayer == (fighterLayer | (1 << collision.gameObject.layer)))
        {
            Hit(collision);
        }
    }

    /// <summary>
    /// takes the collision with an enemy, if the enemy is faster than him it means he is being hit and will be bounced
    /// else,  the enemy will take the same action and the object will slow down to prevent it rushing non stop, it has to slow down after hitting
    /// </summary>
    /// <param name="collision"></param>



    public void Hit(Collision collision)
    {
        // it does the same logic on both script types, only  in navmesh, there is navmesh cancellation logic to hit and push out the arena
        //or else, navmeshes cannot be pushed out of arena
        if (collision.collider.gameObject.TryGetComponent<PlayerScript>(out PlayerScript playerScript))
        {
            Vector3 enemyVelocity = playerScript.GetMovementVelocity();
            if (enemyVelocity.magnitude > agent.velocity.magnitude)
            {
                //I won't handle the enemyvelocity.magnitude < movevelocity.magnitude condition because it automatically
                //will be handled in enemy's script
                Vector3 endposition = transform.position + hitBounceConstant * enemyVelocity.normalized;

                if (!isLerping)
                {
                    StartCoroutine(LerpPosition(hitAnimationDuration,transform.position,endposition));
                }
            }
            else if (enemyVelocity.magnitude < agent.velocity.magnitude)
            {
                agent.velocity = agent.velocity / 4;
                //basically if you hit faster than enemy, it adds point proportional to how fast you were
                AddPoints(Mathf.FloorToInt(scoreMultiplier * agent.velocity.magnitude));
            }
        }
        else if (collision.collider.gameObject.TryGetComponent<Navmesh>(out Navmesh script))
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
            else if (enemyVelocity.magnitude < agent.velocity.magnitude)
            {
                agent.velocity = agent.velocity / 4;
                //basically if you hit faster than enemy, it adds point proportional to how fast you were
                AddPoints(Mathf.FloorToInt(scoreMultiplier * agent.velocity.magnitude));
                isPushing = true;
            }
            else
            {

            }

        }
    }
    /// <summary>
    /// takes the parameter as a number between 0 and 1 and it changnes the graph of it as a sin fuction
    /// </summary>
    /// <param name="progress"></param>
    /// <returns></returns>
    public float SmoothProgress(float progress)
    {
        progress = Mathf.Lerp(-Mathf.PI/2,Mathf.PI/2,progress);
        progress = Mathf.Sin(progress);
        progress = (progress / 2f) + .5f;
        return progress;
    }

    /// <summary>
    /// The agent's navmesh property is preventing it from falling outside map and dying, therefore when hit, navmesh property will be turned off
    /// and only if the agent manages to stay inside the arena, agent will enable again
    /// </summary>
    private bool isLerping = false;
    IEnumerator LerpPosition(float duration,Vector3 startTransformPosition,Vector3 endTransformPosition)
    {
        isLerping = true;
        agent.enabled = false;

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

        // Lerp is complete, perform the action here
        if (!NavMesh.SamplePosition(endTransformPosition, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
        {

        }
        else
        {
            agent.enabled = true;
        }

        isLerping = false;
    }
    public void Dying()
    {
        playersTransforms.GetComponent<PlayerStorage>().AddScore(lastHitPlayer, 15);

    }

    public Vector3 GetMovementVelocity()
    {
        return agent.velocity;
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
        playersTransforms.GetComponent<PlayerStorage>().AddScore(transform,point);
    }
}
