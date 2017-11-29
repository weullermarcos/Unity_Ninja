using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour {

    private BoxCollider2D playerCollider;

    [SerializeField]
    private BoxCollider2D killerCollider;

	// Use this for initialization
	void Start () {
        playerCollider = GameObject.Find("player").GetComponent<BoxCollider2D>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "player")
        {
            Animator playerAnimator = collision.gameObject.GetComponent<Animator>();
            if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("dead"))
                playerAnimator.SetTrigger("died");
            
        }
    }
}
