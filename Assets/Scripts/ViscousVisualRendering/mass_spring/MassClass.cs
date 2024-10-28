using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassClass {
	public float mass;
	public Vector3 position;
	public Vector3 new_position;
	public Vector3 velocity;
	public Vector3 accel;
	public Vector3 massforce;
	public float deltaTimeSec;
    public bool is_fixed;


	public MassClass(float newMass,Vector3 newPosition,float delta_TimeSec)
	{
		mass = newMass;
		position = newPosition;
		new_position = newPosition;
		deltaTimeSec = delta_TimeSec;

	}

	public void massupdate() {
		accel.x = massforce.x / mass;
		accel.y = massforce.y / mass;
		accel.z = massforce.z / mass;

		velocity.x = (velocity.x + accel.x * deltaTimeSec);
		velocity.y = (velocity.y + accel.y * deltaTimeSec);
		velocity.z = (velocity.z + accel.z * deltaTimeSec);

		new_position.x = new_position.x + velocity.x * deltaTimeSec;
		new_position.y = new_position.y + velocity.y * deltaTimeSec;
		new_position.z = new_position.z + velocity.z * deltaTimeSec;


	}

	public void massaddForce (Vector3 newforce) {
		massforce.x = massforce.x+newforce.x;
		massforce.y = massforce.y+newforce.y;
		massforce.z = massforce.z+newforce.z;
//		massforce.x = newforce.x;
//		massforce.y = newforce.y;
//		massforce.z = newforce.z;

	}
		

		

}
