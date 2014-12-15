using UnityEngine;
using System.Collections;

public class SkyLerp : MonoBehaviour {
	public Material material1;
	public Material material2;
	public float duration = 2.0F;

	// Update is called once per frame
	void Start () {
		//RenderSettings.skybox = material2;
	}
	void Update () {
		float lerp = Mathf.PingPong(Time.time, duration) / duration;
		RenderSettings.skybox.Lerp(material1, material2, -1f);
	}
}
