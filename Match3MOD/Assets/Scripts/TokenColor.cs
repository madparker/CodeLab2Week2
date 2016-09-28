using UnityEngine;
using System.Collections;

public class TokenColor : MonoBehaviour {

	public Color color1 = Color.red;
	public Color color2 = Color.blue;
	public float duration = 2.0F;

	SpriteRenderer spriteRenderer;

	void Start() {

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		float t = Mathf.PingPong(Time.time, duration) / duration;
		spriteRenderer.color = Color.Lerp(color1, color2, t);
	}
}
