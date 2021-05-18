using UnityEngine;
using System.Collections;
using ReView;

public class ReViewManager : MonoBehaviour 
{
	public static int Now
	{
		get
		{
			return (int)(Time.time * 1000);
		}
	}
	
	public static int Delta
	{
		get
		{
			return (int)(Time.deltaTime * 1000);
		}
	}
	
	void Start() 
	{
		ReViewFeedManager.Instance.Connect(ReViewStorageServerAddress, ReViewStorageServerPort);
	}
	
	void Update() 
	{
		ReViewFeedManager.Instance.Update(Now, Delta);
	}

	public string ReViewStorageServerAddress = "localhost";
	public int ReViewStorageServerPort = 5000;
}
