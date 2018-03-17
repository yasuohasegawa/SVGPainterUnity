using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGPainterUnity;

public class Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var svgPainter = GetComponent<SVGPainter> ();
		svgPainter.Init ("test.svg");
		//svgPainter.Play (3f, PainterEasing.EaseInOutCubic);


		svgPainter.Play (3f, PainterEasing.EaseInOutCubic, () => {
			
			Debug.Log("Complete");

			svgPainter.Rewind (3f, PainterEasing.EaseInOutCubic, () => {
				Debug.Log("Rewind Complete");
			});
		});

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
