﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	private static Player instance;
	public static Player Instance
	{ 
		get
		{
			if (instance == null) 
			{
				instance = GameObject.FindObjectOfType<Player> ();
			}
			return instance;
		}
	}

	private Animator myAnimator;
	[SerializeField]
	private Transform knifePos;

	private bool facingRight;
	//Serialized field can be edited from the inspector window
	[SerializeField]
	private Transform[] groundPoints;
	[SerializeField]
	private float groundRadius;
	[SerializeField]
	private LayerMask whatIsGround;
	[SerializeField]
	private bool airControl;
	[SerializeField]
	private float jumpForce;
	[SerializeField]
	private GameObject knifePrefab;

	//properties are written different to variables starting with capital letter
	public Rigidbody2D MyRigidbody { get; set;}
	public bool Jump { get; set;}
	public bool OnGround{ get; set;}
	public bool IsRunning{ get; set;}
	[SerializeField]
	public float WalkingSpeed{ get; set;}


	// Use this for initialization
	void Start () {
		
		facingRight = true;
		IsRunning = false;
		WalkingSpeed=4;
		MyRigidbody = GetComponent<Rigidbody2D> ();
		myAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update(){
		HandleInput ();
	}




	void FixedUpdate () 
	{
		float horizontal = Input.GetAxis ("Horizontal");
		OnGround = IsGrounded ();
		HandleMovement (horizontal);
		Flip (horizontal);
		HandleLayers();
		ResetValues ();
	}

	//handles the movement of player
	private void HandleMovement(float horizontal)
	{

		if (MyRigidbody.velocity.y < 0) 
		{
			myAnimator.SetBool ("land", true);
		}
		if (OnGround || airControl) {
			if (IsRunning && Mathf.Abs(horizontal)>0.01)
				/* checks if the running button is pressed or not and turns
				the run animation only when both shift and direction keys are pressed*/
			{ 
				MyRigidbody.velocity = new Vector2 (horizontal*WalkingSpeed*2, MyRigidbody.velocity.y);
				myAnimator.SetBool ("run", true);
				myAnimator.SetFloat("movementSpeed", WalkingSpeed*2);
				/*speed is case sensitive and horizontal is made positive to compare
				player goes right if speed is greater than 0.01 and left if less than 0.01
				checks horizontal to see if the player pressed left or right button
				speed is greater than 0.01 if the speed if player presses right and vice versa*/
			} else {
				MyRigidbody.velocity = new Vector2 (horizontal*WalkingSpeed, MyRigidbody.velocity.y);
				myAnimator.SetBool ("run", false);
				myAnimator.SetFloat("movementSpeed", WalkingSpeed);

			}
		}

		if (Jump && MyRigidbody.velocity.y == 0) {
			MyRigidbody.AddForce (new Vector2 (0, jumpForce));
		}

		myAnimator.SetFloat ("speed", Mathf.Abs (horizontal));
	}

	//Handles the input keyss
	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Space)) //GetKeyDown When key is pressed once
		{
			myAnimator.SetTrigger ("jump");
		}
		if(Input.GetKey(KeyCode.LeftShift)){ //GetKey Checks when key is pressed continuously
			IsRunning = true;
		}

		if (Input.GetKeyDown (KeyCode.V)) {
			myAnimator.SetTrigger ("throw");
		}
	}



	//Flips the player in opposite direction
	private void Flip(float horizontal)
	{
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight; //toggles the values true if its false and vice versa.
			Vector3 theScale = transform.localScale; //referencing the local scale of player
			theScale.x*=-1; //multiplying the value of x of scale by negative
			transform.localScale = theScale; //changed the value of scale in Transform
		}
	}



	//Checks if the player is standing on the ground
	private bool IsGrounded()
	{
		if (MyRigidbody.velocity.y <= 0)
		{
			foreach (Transform point in groundPoints)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll (point.position, groundRadius, whatIsGround);

				for (int i = 0; i < colliders.Length; i++)
				{
					if (colliders [i].gameObject != gameObject) 
					{	
						return true;
					}
				}
			}	
		}
		return false;
	}

	//resets the values
	private void ResetValues(){
		IsRunning = false;
	}
		
	//Handles the animator layers
	private void HandleLayers()
	{
		if (!OnGround) {
			myAnimator.SetLayerWeight (1, 1); //Layer weight is set to 1 and Layer number 1 refers to AirLayer
		}else{
			myAnimator.SetLayerWeight (1, 0); //0 refers to GroundLayer
		}
	}

	//Throws knife
	public void ThrowKnife(int value)
	{
		//makes sure we only throw one knife at a time
		if (!OnGround && value == 1 || OnGround && value == 0) 
		{
			if (facingRight)
			{

				GameObject tmp = (GameObject)Instantiate (knifePrefab, knifePos.position , Quaternion.Euler(new Vector3(180,0,-90))); // rotates the knife
				tmp.GetComponent<Knife>().Initialize(Vector2.right);
			} else 
			{
				GameObject tmp = (GameObject)Instantiate (knifePrefab, knifePos.position, Quaternion.Euler(new Vector3(0,0,90)));
				tmp.GetComponent<Knife>().Initialize(Vector2.left);
			}
		}

	}

}
