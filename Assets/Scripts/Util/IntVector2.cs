using System;
using UnityEngine;

public class IntVector2 {
	public int x;
	public int y;
	
	public IntVector2(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public override string ToString () {
		return "(" + x + ", " + y + ")";
	}

    public IntVector2(Vector3 vector3)  {
        x = Mathf.FloorToInt(vector3.x);
        y = Mathf.FloorToInt(vector3.z);
    }
}


