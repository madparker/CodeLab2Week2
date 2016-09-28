using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour {

	private Vector3 originPosition;
	private Quaternion originRotation;
	public float shake_decay = 0f;
	public float shake_intensity = 0f;

	private float temp_shake_intensity = 0;

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		if (temp_shake_intensity > 0){
			transform.position = originPosition + Random.insideUnitSphere * temp_shake_intensity;
			transform.rotation = new Quaternion(
				originRotation.x + Random.Range (-temp_shake_intensity,temp_shake_intensity) * 0f,
				originRotation.y + Random.Range (-temp_shake_intensity,temp_shake_intensity) * 0f,
				originRotation.z + Random.Range (-temp_shake_intensity,temp_shake_intensity) * 0f,
				originRotation.w + Random.Range (-temp_shake_intensity,temp_shake_intensity) * 0f);
			temp_shake_intensity -= shake_decay;
		}

		RandomShake ();
	}

	void ShakeToken(){
		originPosition = transform.position;
		originRotation = transform.rotation;
		temp_shake_intensity = shake_intensity;

	}

	void RandomShake(){
	
		float randTime = Random.Range (2, 5);
		Invoke("ShakeToken", randTime);
		StopShake ();
	}

	void StopShake(){
	
		Invoke ("DestroyScript", 2.5f);
	}

	void DestroyScript(){
		Component shakeScript = GetComponent<Shake>();
		Destroy (shakeScript);
	}
}
