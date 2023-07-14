using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    private Animator animator;
    private const string IS_WALKING = "IsWalking";
    private const string IS_SLOWING = "IsSlowing";
    private const string IS_PUSHING = "IsPushing";

    [SerializeField] private PlayerScript player;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_SLOWING, player.IsSlowing());
        animator.SetBool(IS_PUSHING, player.IsPushing());
    }
}
