using UnityEngine;

public class ColliderVisualizer : MonoBehaviour
{
	//private void OnDrawGizmos()
	//{
	//	Collider collider = GetComponent<Collider>();
	//	if (collider != null)
	//	{
	//		Gizmos.color = Color.red;
	//		Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
	//	}
	//}

	void Awake()
	{
		Collider collider = GetComponent<Collider>();

		Debug.LogWarning(collider.bounds);
		
		if (collider != null)
		{
			Vector3 center = collider.bounds.center;
			Vector3 size = collider.bounds.size;

			Debug.Log(center);
			Debug.Log(size);
			
			// Draw the bounds of the collider
			Vector3 v3FrontTopLeft = center + new Vector3(-size.x, size.y, size.z) * 0.5f;
			Vector3 v3FrontTopRight = center + new Vector3(size.x, size.y, size.z) * 0.5f;
			Vector3 v3FrontBottomLeft = center + new Vector3(-size.x, -size.y, size.z) * 0.5f;
			Vector3 v3FrontBottomRight = center + new Vector3(size.x, -size.y, size.z) * 0.5f;
			Vector3 v3BackTopLeft = center + new Vector3(-size.x, size.y, -size.z) * 0.5f;
			Vector3 v3BackTopRight = center + new Vector3(size.x, size.y, -size.z) * 0.5f;
			Vector3 v3BackBottomLeft = center + new Vector3(-size.x, -size.y, -size.z) * 0.5f;
			Vector3 v3BackBottomRight = center + new Vector3(size.x, -size.y, -size.z) * 0.5f;
			
			// Lines for each edge
			Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, Color.red);
			Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, Color.red);
			Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, Color.red);
			Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, Color.red);

			Debug.DrawLine(v3BackTopLeft, v3BackTopRight, Color.red);
			Debug.DrawLine(v3BackTopRight, v3BackBottomRight, Color.red);
			Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, Color.red);
			Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, Color.red);

			Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, Color.red);
			Debug.DrawLine(v3FrontTopRight, v3BackTopRight, Color.red);
			Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, Color.red);
			Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, Color.red);
		}
	}
}
