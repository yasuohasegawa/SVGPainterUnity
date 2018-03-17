using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using SVGPainterUnity;

public class ToPointsTest {

	[Test]
	public void ParseTest() {
		string d = "M2,173.3c0,47.3,19.2,90.1,50.2,121.1s73.8,50.2,121.1,50.2s90.1-19.2,121.1-50.2s50.2-73.8,50.2-121.1\n\ts-19.2-90.1-50.2-121.1S220.6,2,173.3,2S83.2,21.2,52.2,52.2S2,126,2,173.3z";

		List<string> testCommands = new List<string> (){ "M", "c", "s", "s", "s", "s", "S", "S", "S", "z" };

		List<float> testPoints = new List<float> () {
			2.0f,
			173.3f,
			52.2f,
			294.4f,
			173.3f,
			344.6f,
			294.4f,
			294.4f,
			344.6f,
			173.3f,
			294.4f,
			52.2f,
			173.3f,
			2.0f,
			52.2f,
			52.2f,
			2.0f,
			173.3f
		};

		ToPoints svgPoints = new ToPoints ();

		Assert.AreEqual (testCommands, svgPoints.GetCommands (d));

		List<PointData> test = svgPoints.GetPointsFromPath (d).svgPoints;
		List<float> resPoints = new List<float> ();

		for (int i = 0; i < test.Count; i++) {
			resPoints.Add (float.Parse(test [i].pos.x.ToString("N1")));
			resPoints.Add (float.Parse(test [i].pos.y.ToString("N1")));
		}

		Assert.AreEqual (testPoints, resPoints);
	}
}
