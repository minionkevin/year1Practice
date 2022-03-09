using UnityEngine;
using System.Collections;

public class DropShadow : MonoBehaviour {

    public Vector3 offsetPosition;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
  
    }

    void LateUpdate()
    {
        if(transform.parent)
         {
            Vector3 offset = new Vector3(transform.parent.position.x + offsetPosition.x, transform.parent.position.y + offsetPosition.y, offsetPosition.z);
            //get the world position of the parent and set the offset 
            transform.position = offset;
        }
    }
}
