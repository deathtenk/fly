using UnityEngine;
using System.Collections;

public class DataController : MonoBehaviour {

	public static DataController dc;

	public string site;
	public string username;
	public string password;
	public float sealevel;
	public bool hasGravity;

	void Awake() {
		if (dc == null)
		{
			DontDestroyOnLoad(gameObject);
			dc = this;
		}
		else if (dc != this)
		{
			Destroy(gameObject);
		}

		sealevel = GameObject.FindGameObjectWithTag("Water").transform.position.y;
	}
}
