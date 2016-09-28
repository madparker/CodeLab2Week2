using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	public GameObject OMG;

	void Start () {
	
		Destroy (OMG, 2f);
	}
	
}