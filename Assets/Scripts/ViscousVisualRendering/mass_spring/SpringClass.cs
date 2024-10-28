using UnityEngine;
using System;

public class SpringClass {
	public MassClass m1;
	public MassClass m2;
	public Vector3 springvector;
	public Vector3 springvector_n;
	public Vector3 springdir;
	public float original_spring_length;
	public float new_spring_length;
	public float threashold;
	public bool cutted = false;

	public SpringClass(MassClass newMass1, MassClass newMass2,float t_hold)
	{
		m1 = newMass1;
		m2 = newMass2;
		threashold = t_hold;
	}

	public void updateValues() {
		springvector.x = m1.position.x - m2.position.x;
		springvector.y = m1.position.y - m2.position.y;
		springvector.z = m1.position.z - m2.position.z;

		springvector_n.x = m1.new_position.x - m2.new_position.x;
		springvector_n.y = m1.new_position.y - m2.new_position.y;
		springvector_n.z = m1.new_position.z - m2.new_position.z;

		original_spring_length = (float)Math.Sqrt (springvector.x * springvector.x + springvector.y * springvector.y + springvector.z * springvector.z);
		new_spring_length = (float)Math.Sqrt (springvector_n.x * springvector_n.x + springvector_n.y * springvector_n.y + springvector_n.z * springvector_n.z);

		springdir.x = springvector_n.x / new_spring_length;
		springdir.y = springvector_n.y / new_spring_length;
		springdir.z = springvector_n.z / new_spring_length;



	}

	public void cut() {
		cutted = true;
		
	}

}
