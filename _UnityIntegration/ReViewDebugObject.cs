using UnityEngine;
using System.Collections;
using ReView;

public class ReViewDebugObject : ReViewFeedObject
{
	public ReViewDebugObject(ReViewDebugObject inParentDebugObject, string inDebugName, EDebugType inType) : base(inParentDebugObject, inDebugName, ReViewManager.Now, inType)
	{
	}

	public ReViewDebugObject End()
	{
		EndBlock(ReViewManager.Now);
		return this;
	}

	public ReViewDebugObject Log(string message)
	{
		Log(message, ReViewManager.Now, 1);
		return this;
	}

	public ReViewDebugObject LogWarning(string message)
	{
		Log(message, ReViewManager.Now, 2);
		return this;
	}

	public ReViewDebugObject LogError(string message)
	{
		Log(message, ReViewManager.Now, 4);
		return this;
	}
}
