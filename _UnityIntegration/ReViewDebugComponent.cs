using UnityEngine;
using System.Collections;
using ReView;

public class ReViewDebugComponent : MonoBehaviour 
{
	void Start()
	{
		GameObject parentGameObject = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : null;
		ReViewDebugComponent parent = parentGameObject != null ? parentGameObject.GetComponent<ReViewDebugComponent>() : null;
		ReViewDebugObject = new ReViewDebugObject(parent != null ? parent.ReViewDebugObject : null, name, ReViewDebugObject.EDebugType.Track);
		
		ReViewDebugObject.Log("Added debug component " + name + " for gameobject " + gameObject.name);
	}
	
	void LateUpdate()
	{
	
	}
	
	public ReViewDebugObject ReViewDebugObject
	{
		get
		{
			return reViewDebugObject;
		}
		private set
		{
			reViewDebugObject = value;
		}
	}
	
	private ReViewDebugObject reViewDebugObject = null;
}
