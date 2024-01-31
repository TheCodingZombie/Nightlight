using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class Ally : MonoBehaviour
{
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
	public float life = 10;

	public bool facingRight = true;

	public float speed = 5f; 

	public bool isInvincible = false;
	private bool isHitted = false;

	private float dir = 1;

	[SerializeField] private float m_DashForce = 25f;
	public float meleeDist = 1.5f;
	public float rangeDist = 5f;
	private bool canAttack = true;
	private Transform attackCheck;
	private Transform m_WallCheck;

	public float dmgValue = 1;
	public GameObject player;
	private bool follow = true; // dictates whether or not the candle will stay by the player
	public bool inWall = false; // if you put the candle in a wall, you may not attack it.
	private bool RecallCooldown = false; // press C to recall it back to you.
	private bool HitSomeone = false; // becomes true if it hits something, returns to false if it gets hit again or returns

	void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		attackCheck = transform.Find("AttackCheck").transform;
		m_WallCheck =  transform.Find("WallCheck").transform;
		m_Rigidbody2D.gravityScale = 1;
	}

	void Update()
	{	
		if (Input.GetKeyDown(KeyCode.C) && (!RecallCooldown))
		{
			RecallCooldown = true;
			follow = true;
			StartCoroutine(Recall());
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		bool wasWall = inWall;
		inWall = false;
		CharacterController2D charCon = player.GetComponent<CharacterController2D>();
		if (life <= 0)
		{
			StartCoroutine(DestroyEnemy());
		}

		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, charCon.m_WhatIsGround);
		dmgValue = Mathf.Abs(dmgValue);
		
		int LayerIgnoreRaycast = LayerMask.NameToLayer("Front");
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.layer != LayerIgnoreRaycast){
				inWall = true;
			}

		}
		if (!follow && !inWall){
			Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
			for (int i = 0; i < collidersEnemies.Length; i++)
			{
				if ((collidersEnemies[i].gameObject.tag == "Enemy") && (collidersEnemies[i].gameObject != gameObject) && !HitSomeone){
					if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
					{
						dmgValue = -dmgValue;
					}
					collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
					HitSomeone = true;
					transform.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * transform.gameObject.GetComponent<Rigidbody2D>().velocity.x, transform.gameObject.GetComponent<Rigidbody2D>().velocity.y);
				}
			}
		}

		// constantly follow the player
		if (follow){
			transform.position = new Vector2(player.transform.position.x + (0.75f * dir), player.transform.position.y);
			transform.gameObject.GetComponent<Rigidbody2D>().rotation = 0f;
			m_Rigidbody2D.gravityScale = 0;
			HitSomeone = false;
		}
		else{
			m_Rigidbody2D.gravityScale = 1;
		}

		
		m_FacingRight = charCon.m_FacingRight;

		if (facingRight != m_FacingRight){
			facingRight = m_FacingRight;
			dir *= -1;

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}
	public void ApplyDamage(float damage)
	{
		if (!inWall){
			follow = false;
			HitSomeone = false;
			float direction = damage / Mathf.Abs(damage);
			transform.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			transform.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(damage * 50f, direction * damage * 50f)); 
			StartCoroutine(HitTime());
		}
	}

	public void Idle()
	{
		m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);
	}

	IEnumerator HitTime()
	{
		isInvincible = true;
		yield return new WaitForSeconds(5.0f);
		follow = true;
		isHitted = false;
	}

	IEnumerator DestroyEnemy()
	{
		CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
		capsule.size = new Vector2(1f, 0.25f);
		capsule.offset = new Vector2(0f, -0.8f);
		capsule.direction = CapsuleDirection2D.Horizontal;
		yield return new WaitForSeconds(0.25f);
		m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
		yield return new WaitForSeconds(1f);
		Destroy(gameObject);
	}

	IEnumerator Recall()
	{
		yield return new WaitForSeconds(3.0f);
		RecallCooldown = false;
	}
}
