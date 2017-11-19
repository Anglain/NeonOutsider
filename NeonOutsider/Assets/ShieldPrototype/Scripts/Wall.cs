using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IRewindable 
{
    public float duration;
    public float distanceFromPlayer;
    
    void Start()
    {
        
	//    Place(new Vector3(0, 0, 0), new Vector3(2, 1, 0));
    }

    public void Place(Vector3 playerPos, Vector3 mousePos)
    {
        playerPos.z = 0f;
        mousePos.z = 0f;    

        Vector3 dir = Vector3.Normalize(mousePos - playerPos);
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);
        dir /= 2;
        // dir.Normalize();
        
        Debug.Log(dir);

        Vector3 displacementVec = dir * distanceFromPlayer;
        gameObject.transform.position = playerPos + displacementVec;
        
        gameObject.transform.up = - dir;
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
