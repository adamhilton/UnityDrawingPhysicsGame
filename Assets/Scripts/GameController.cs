using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	public GameObject ballPrefab;
	public Camera camera;

	private List<GameObject> allSegments = new List<GameObject> ();

	private GameObject previousSegment;

	void Start ()
	{
	}

	void Update ()
	{
		if (DrawBegin ()) {
			GameObject prefab = CreateBallPrefab(GetWorldPositionFromMousePosition());
			
			previousSegment = prefab;
			allSegments.Add (prefab);

		} else if (DrawIsInProgress ()) { 
			Vector3 currentPosition = camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));

			if (currentPosition.Equals (previousSegment.transform.position)) {
				return;
			}

			GameObject prefab = CreateBallPrefab(currentPosition);
			
			FixedJoint2D joint = prefab.AddComponent<FixedJoint2D> ();
			joint.connectedBody = previousSegment.GetComponent<Rigidbody2D> (); 

			previousSegment = prefab;
			allSegments.Add (prefab);
			
		} else if (DrawEnded ()) {
			StartCoroutine ("ActivateSegments");
		}
	}

	private GameObject CreateBallPrefab (Vector3 position)
	{
		return (GameObject)Instantiate (ballPrefab, position, Quaternion.identity);
	}

	private Vector3 GetWorldPositionFromMousePosition ()
	{
		return camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
	}

	IEnumerator ActivateSegments ()
	{
		foreach (GameObject gameObject in allSegments) {
			gameObject.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
		}

		allSegments = new List<GameObject> ();

		yield return null;
	}

	static bool DrawBegin ()
	{
		return (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) || Input.GetMouseButtonDown (0);
	}

	static bool DrawIsInProgress ()
	{
		return ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0));
	}

	static bool DrawEnded ()
	{
		return (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0);
	}
}