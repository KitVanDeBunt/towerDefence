﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	private PathManager pathMngr;
	private EnemyManager enemyMngr;
	private Vector3[] path;
	private int ProgressInPath = 0;
	private float DistPointReached = 1.5f;
	private Vector3 target;
	private bool pathFound;
	
	public float maxAngularVelocity = 2;
	public float drag = 0.97f;
	public float dashForce = 200;
	public float moveForce = 2;
	public float maxRotateForce = 1;
	[SerializeField]
	private int startHealt;

	private Healt healt;

	private void Start () {
		healt = new Healt(startHealt);

		pathMngr = GameObject.Find("pathManager").GetComponent<PathManager>() as PathManager;
		enemyMngr = GameObject.Find("gameManager").GetComponent<EnemyManager>() as EnemyManager;
		getTarget();
		transform.rotation =  movement.RotateToPoint(transform,target);
	}

	private void FixedUpdate(){
		float distanceToTarget = Vector3.Distance(transform.position,target);
		if(pathFound){

			if(DistPointReached>distanceToTarget){
				ProgressInPath++;
				if(ProgressInPath>path.Length-1){
					getTarget();
				}else{
					target = path[ProgressInPath];
				}
			}
		}else{
			if(DistPointReached>distanceToTarget){
				enemyMngr.removeEnemy(transform.gameObject);
			}
		}

		rigidbody2D.AddForce( movement.ForceAndAngleToDirection(moveForce,transform.rotation.eulerAngles.z));
		Drag();
		RotateToPoint();
		//transform.rotation =  Quaternion.Euler(new Vector3(0, 0, movement.RotateToPoint(transform,target)));

	}

	private void getTarget(){
		path = pathMngr.getRandomPath(transform.position,5);
		ProgressInPath = 0;
		if(path!=null){
			pathFound = true;
			target = path[ProgressInPath];
		}else{
			pathFound = false;
			target = enemyMngr.getClosestEnd(transform.position);
		}
	}

	private void RotateToPoint(){
		rigidbody2D.AddTorque(movement.RotateForce(transform.position,
		                                           transform.eulerAngles.z,
		                                           target,
		                                           maxRotateForce,
		                                           1));
		//limit force
		rigidbody2D.angularVelocity = movement.limitTorque(rigidbody2D.angularVelocity,maxAngularVelocity);
	}

	private void oldRotateToPoint(){
		float thisRotation =  transform.eulerAngles.z;
		float deltaY = transform.position.y - target.y;
		float deltaX = transform.position.x - target.x;
		float rotationGoal = (Mathf.Atan2(deltaY,deltaX) * 180 / Mathf.PI)+90;
		//transform.rotation =  Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
		float deltaAngel = thisRotation - rotationGoal;
		//Debug.Log(thisRotation +"-"+rotationGoal+"="+deltaAngel);
		if(deltaAngel>360){
			deltaAngel -=360;
		}

		//if(deltaAngel<0){
		//	deltaAngel +=360;
		//}
		if(deltaAngel>maxRotateForce){
			deltaAngel = maxRotateForce;
		}else if(deltaAngel<maxRotateForce){
			deltaAngel = -maxRotateForce;
		}
		//Debug.Log(deltaAngel);
		//Debug.Log(path[ProgressInPath]);
		//get input
		//add force
		rigidbody2D.AddTorque(-deltaAngel);
		//limit force
		if(rigidbody2D.angularVelocity > maxAngularVelocity){
			rigidbody2D.angularVelocity = maxAngularVelocity;
		}else if(rigidbody2D.angularVelocity < -maxAngularVelocity){
			rigidbody2D.angularVelocity = -maxAngularVelocity;
		}
	}

	private void Drag(){
		float newX = 0;
		float newY = 0;
		if(rigidbody2D.velocity.x!=0){
			newX = rigidbody2D.velocity.x*drag;
		}else{
			newX = rigidbody2D.velocity.x;
		}
		if(rigidbody2D.velocity.y!=0){
			newY= rigidbody2D.velocity.y*drag;
		}else{
			newY= rigidbody2D.velocity.y;
		}
		rigidbody2D.velocity = new Vector2(newX,newY);
	}

	private void OnTriggerEnter2D(Collider2D col){
		if((itemType)col.gameObject.GetComponent<Item>().type == itemType.Bullet){
			healt.ChangeHealt(-10);
			if(healt.dead){
				enemyMngr.removeEnemy(transform.gameObject);
			}
		}
	}
}