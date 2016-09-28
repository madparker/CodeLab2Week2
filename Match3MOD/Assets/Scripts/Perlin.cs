using UnityEngine;
using System.Collections;

public class Perlin : MonoBehaviour {

	public GameObject OMG;

	public float heightScale = Random.Range(0.0f,1.0F);
	public float xScale = Random.Range(0.0f,1.0F);
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		float height = heightScale * Mathf.PerlinNoise(Time.time * xScale, 0.0F);
		float width = heightScale * Mathf.PerlinNoise(Time.time * xScale, 0.0F);
		Vector3 pos = transform.position;
		pos.y = height;
		pos.x = width;
		transform.position = pos;
	}
}
