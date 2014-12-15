using UnityEngine;
using System.Collections;
//using Faker;

public class GameController : MonoBehaviour {
	
	public string email;
	public string name;


	public PlanePilot pilot;
	public bool crashed;

	void Start()
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<TextMesh>().text = 
				"a button to speed up \n" +
				"b button slow down \n" +
				"left stick to pitch and roll\n" +
				"LB and RB to yaw\n";

        pilot = GameObject.FindGameObjectWithTag("Player").GetComponent<PlanePilot>();
		name = Faker.Name.FullName();
		email = Faker.Internet.Email();
	}

	void OnGUI() {
		if (pilot.enabled == false && crashed == false)
		{
			DataController.dc.hasGravity = GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 2 + 200, 200, 20), DataController.dc.hasGravity, "gravity");
			DataController.dc.site = GUI.TextField(new Rect(Screen.width / 2, Screen.height / 2 - 80, 200, 20), DataController.dc.site, 128);
			DataController.dc.username = GUI.TextField(new Rect(Screen.width / 2, Screen.height / 2 - 60, 200, 20), DataController.dc.username, 128);
			DataController.dc.password = GUI.PasswordField(new Rect(Screen.width / 2, Screen.height / 2 - 40, 200, 20), DataController.dc.password, "*"[0], 128);
			name = GUI.TextField(new Rect(Screen.width / 2, Screen.height / 2 - 20, 200, 20), name, 128);
			email = GUI.TextField(new Rect(Screen.width / 2, Screen.height / 2, 200, 20), email, 128);

			if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 35, 200, 30), "Start Simulation (Press A)") || Input.GetKey(KeyCode.Joystick1Button1))
				pilot.enabled = true;
		}

		if (pilot.enabled == false && crashed == true)
		{
			if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 35, 200, 30), "Restart (Press A)") || Input.GetKey(KeyCode.Joystick1Button1))
				Application.LoadLevel(0);
		}
	}
}
