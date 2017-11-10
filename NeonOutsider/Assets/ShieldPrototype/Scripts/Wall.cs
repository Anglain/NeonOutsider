using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IRewindable 
{
   void Start()
   {
	   
   }

	#region IRewindable members

    public void Rewind()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 UsagePosition()
    {
        throw new System.NotImplementedException();
    }

	 public void Dispose()
    {
        throw new System.NotImplementedException();
    }

	#endregion

}
