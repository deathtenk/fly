using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;
using RestSharp;

namespace XAPILib
{
	
    public class Statement {
		public string id{get; private set;}
		public Hashtable actor{ get; set;}
		public Hashtable verb{get; set;}
		public Hashtable obj{get; set;} 
		public Hashtable context{get; set;}
		public Hashtable statement{get; private set;}

		public Statement()
		{
			//local hashtables

			// generate UUID here
			//id = System.Guid.NewGuid().ToString();
			actor = new Hashtable();
			verb = new Hashtable();
			obj = new Hashtable();
			context = new Hashtable();

			actor.Add("mbox","");
			actor.Add ("objectType","");
			//verb.Add ("display", "");
			verb.Add("id", "");
			obj.Add ("id","");
			obj.Add("objectType", "");
			obj.Add ("definition", new Hashtable());
			context.Add ("extensions", new Hashtable());
			statement = new Hashtable();
		}

		private void ValidateAndStoreActor() {
			if (!actor.ContainsKey("mbox") || actor["mbox"].GetType() != typeof(string)) {
				throw new System.Exception("Actor must contain the key 'mbox' with a value of type 'string'");
			}
			else {
				statement.Add ("actor", actor);
			}
		}

		private bool ValidateVerb()
		{
			if (!verb.ContainsKey("id") || !verb.ContainsKey("display"))
				return false;
			return true;
		}

		private bool ValidateObject()
		{
			if(!obj.ContainsKey("id"))
				return false;
			return true;
		}



		public Hashtable SetStatement()
		{
			Hashtable s = new Hashtable();
			//s.Add ("id", id);
			//ValidateAndStoreActor();
			s.Add ("actor", actor);
			s.Add ("verb", verb);
			s.Add ("object", obj);
			s.Add ("context", context);
			return s;
		}

		public class DigestAuthenticator : IAuthenticator
		{
			private readonly string _user;
			private readonly string _pass;
					
			public DigestAuthenticator(string user, string pass)
			{
						_user = user;
						_pass = pass;
			}
					
					public void Authenticate(
						IRestClient client, IRestRequest request)
			{
						request.Credentials = new NetworkCredential(_user, _pass);
			}
		}

		public static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		// this jiggery pokery saves the json to the given pathname
		// at the given filename
		public void SaveStatement(string site, string route, string name, string password)
		{
			Hashtable state = new Hashtable();
			state = SetStatement();
			//pathName = Application.dataPath + "/../"+ pathName + "/";
			string data = JsonWriter.Serialize(state);
			var client =  new RestClient(site);
			//String comb = "123456789:123456789";
			string comb = name + ":" + password;
			String auth = Base64Encode(comb);
			//Debug.Log(auth.ToString());

            var request = new RestRequest(route, Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader("Accept", "application/json");
			request.AddHeader("X-Experience-API-Version", "1.0.1");
			request.AddHeader("Authorization", "Basic " + auth);
			request.AddParameter("application/json", data, ParameterType.RequestBody);
			try
			{
				client.ExecuteAsync(request, response =>
				    {
						//Debug.Log ("After Execution");
						if (response.StatusCode == System.Net.HttpStatusCode.OK)
						{
							//Debug.Log (response.StatusCode.ToString());
						}
						else
						{
							//Debug.Log (response.StatusCode.ToString());
						}
					});
			}
			catch (Exception error)
			{
				Debug.Log (error.Message);
			}
			//if(!Directory.Exists (pathName)) {
			//	Directory.CreateDirectory(pathName);
            //}
			//var streamWriter = new StreamWriter(pathName + fileName + ".txt");
			//streamWriter.Write (data);
			//streamWriter.Close();
			//Debug.Log ("Successfully Saved!");
		}
	}

 
}