using UnityEngine;
using System.Collections;
using XAPILib;

public class PlanePilot : MonoBehaviour {
	public float maxSpeed = 90.0f;
	public float minSpeed = 100.0f;
	public float speed = 0.0f;
	public float acceleration = 1.0f;
	public float gravity = 120f;
	public GameObject UI;
	public float velocity = 0;

	public float timeToSend = 5.0f;
	private float sendReset;

	Vector3 lastPosition = Vector3.zero;
	Statement positionStatement = new Statement();

	private float lastUpdateTime = 0;
	public bool crashed = false;
	// Use this for initialization

	public GameController gc;
	void Start () {
		// find local objects
		gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		UI = GameObject.FindGameObjectWithTag("UI");
		sendReset = timeToSend;

		// set verb

		// set statement
		positionStatement.actor["name"] = gc.name;
		positionStatement.actor["mbox"] = "mailto:" + gc.email;
		positionStatement.actor["objectType"] = "Agent";

		// set object
		positionStatement.obj["objectType"] = "Activity";
		positionStatement.obj["id"] = "http://xapidefs.yetanalytics.com/activities/sim-flight";
		Hashtable definition = new Hashtable();
		Hashtable definitionName = new Hashtable();
		Hashtable definitionDescription = new Hashtable();
		definitionName.Add("en-US", "Flying Plane");
		definitionDescription.Add("en-US", "the flying of plane");
		definition.Add("name", definitionName);
		definition.Add("description",definitionDescription);
		positionStatement.obj["definition"] = definition;




		//Debug.Log (s.verb["display"] << );
		}
	
	// Update is called once per frame


	void Update () {

		//gravity
		if (DataController.dc.hasGravity)
			transform.position = new Vector3(transform.position.x, transform.position.y - gravity * Time.deltaTime, transform.position.z);

		//speed
		speed += Input.GetAxis("Jump") * acceleration;
		if (speed > maxSpeed){
			speed = maxSpeed;
		}
		if (speed < minSpeed) {
			speed = minSpeed;
		}
		transform.position += transform.forward * Time.deltaTime * speed;

		//controls
		transform.Rotate (Input.GetAxis ("Vertical"), Input.GetAxis ("Rotational") , -Input.GetAxis ("Horizontal"));


		//ground collision
		float terrainHeight = Terrain.activeTerrain.SampleHeight( transform.position );
		if(terrainHeight > transform.position.y || DataController.dc.sealevel > transform.position.y) {
			SendFlightDataToLRS(transform.position.x, transform.position.z,velocity, 
			                    Mathf.Round(transform.position.y - DataController.dc.sealevel), "crashed");
			gc.crashed = true;
			this.enabled = false;
			/*transform.position = new Vector3(transform.position.x,
			                                 terrainHeight,
			                                 transform.position.z);*/
		}

		//velocity = (transform.position - lastPosition).magnitude;

		float distanceTraveled = MetersToKilometers(Vector3.Distance(transform.position, lastPosition));
		float curTime = Time.time;
		float updateTime = curTime - lastUpdateTime;
		float updateTimeHours = updateTime/60.0f/60.0f;
		velocity = distanceTraveled / updateTimeHours;
		lastPosition = transform.position;

		UI.GetComponent<TextMesh>().text = "velocity = " + velocity + "kph\n"
			+ "altitude = " + Mathf.Round(transform.position.y - DataController.dc.sealevel) + "\n"
		+ "X = " + transform.position.x + "\n"
		+ "Y = " + transform.position.z + "\n";

		// when to send statement
		timeToSend -= Time.deltaTime;
		if (timeToSend < 0f) {
			//Debug.Log(positionStatement.actor["mbox"]);
			timeToSend = sendReset;
			//Debug.Log ("sent!");
			SendFlightDataToLRS(transform.position.x, transform.position.z, velocity,
			                    Mathf.Round(transform.position.y - DataController.dc.sealevel), "aviated");
			//positionStatement.SaveStatement("http://192.168.2.112:9000","xapi/statements");

		}
		lastUpdateTime = curTime;

	}

	void SaveCoordinateData(float x, float y, float velocity, float altitude) {
		Hashtable coordinates = new Hashtable();
		Hashtable extensions = new Hashtable();
		coordinates.Add("x", x);
		coordinates.Add("y", y);
		coordinates.Add("velocity", velocity);
		coordinates.Add("altitude", altitude);
		extensions.Add("http://xapidefs.yetanalytics.com/extensions/coordinates", coordinates);
		positionStatement.context["extensions"] = extensions;
	}
	
	void SendFlightDataToLRS(float x, float y, float velocity, float altitude , string verb) {
		Hashtable display = new Hashtable();
		display.Add("en-US", verb);
		positionStatement.verb["display"] = display;
		positionStatement.verb["id"] = "http://xapidefs.yetanalytics.com/verbs/" + verb;
		SaveCoordinateData(x, y, velocity, altitude);
		positionStatement.SaveStatement("http://" + DataController.dc.site, "xapi/statements", DataController.dc.username, DataController.dc.password);
	}
	

	float MetersToKilometers(float meters) {
		return meters * 0.001f;
	}
	

}
