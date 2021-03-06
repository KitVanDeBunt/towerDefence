﻿using UnityEngine;
using System.Collections;

public class CameraFollow1 : MonoBehaviour
{
	private GameObject cameraTarget;
	
	public float smoothTime = 0.1f;
	public bool cameraFollowX = true;
	public bool cameraFollowY = true;
	
	private Vector2 velocity;
	private Vector3 newPos;
	private float newXPos;
	private float newYPos;
	private Vector3 targetPos;
	
	public float maxXpos;
	public float minXpos;
	public float maxYpos;
	public float minYpos;
	
	void Start(){
		cameraTarget = GameObject.Find("player");
	}
	
	void LateUpdate()
	{
		//get input and mouse position
		Vector3 mouseScreenPos = Input.mousePosition;
		int screenWidth = Screen.width;
		int screenHeight = Screen.height;
		if(mouseScreenPos.x<0){
			mouseScreenPos.x = 0;
		}else if(mouseScreenPos.x>screenWidth){
			mouseScreenPos.x = screenWidth;
		}
		if(mouseScreenPos.y<0){
			mouseScreenPos.y = 0;
		}else if(mouseScreenPos.y>screenHeight){
			mouseScreenPos.y = screenHeight;
		}
		Ray mouseRay = Camera.main.ScreenPointToRay(mouseScreenPos);
		Vector3 mousePosition = new Vector3(mouseRay.origin.x,mouseRay.origin.y,0);
		Vector2 mousePos2D = new Vector2(mouseRay.origin.x,mouseRay.origin.y);
		newPos = transform.position;
		Vector3 distMouseTarget = mousePosition - cameraTarget.transform.position;
		targetPos = (cameraTarget.transform.position + (distMouseTarget/3));
		
		if (cameraFollowX)
		{
			newXPos = Mathf.SmoothDamp(newPos.x, targetPos.x, ref velocity.x, smoothTime);
		}
		if (cameraFollowY)
		{
			newYPos = Mathf.SmoothDamp(newPos.y, targetPos.y, ref velocity.y, smoothTime);
		}
		if(newXPos>maxXpos){
			newXPos = maxXpos;
		}else if(newXPos<minXpos){
			newXPos = minXpos;
		}
		if(newYPos>maxYpos){
			newYPos = maxYpos;
		}else if(newYPos<minYpos){
			newYPos = minYpos;
		}
		//Update camera position
		newPos = new Vector3(newXPos,newYPos,newPos.z);
		transform.position = newPos;
	}
}
