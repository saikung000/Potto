using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class MoveToTargetScript : MonoBehaviour {
	public Transform TargetPosition;
	Camera mainCamera;
	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
	}
	
	public void GoToPosition(){

		if (TargetPosition != null) {
			mainCamera.GetComponent<ProCamera2D> ().AddCameraTarget (TargetPosition);
		}

	}

	public void GoToPosition(Transform target){

		TargetPosition = target;
		if (TargetPosition != null) {
			mainCamera.GetComponent<ProCamera2D> ().AddCameraTarget (TargetPosition);
		}

	}

	public void removePosition(){
		mainCamera.GetComponent<ProCamera2D> ().RemoveAllCameraTargets ();


	}
}
