using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using SVGPainterUnity;

public class SVGDataParserTest {

	[Test]
	public void LoadTest() {
		var parserObj = new GameObject("parser");
		var parser = parserObj.AddComponent<SVGDataParser> ();
		parser.Load ("test.svg");
		parser.GetSize ();

		Assert.AreEqual (new Vector2 (352f, 230f), parser.GetSize ());
		Assert.AreEqual (10, parser.GetPaths().Count);
	}
}
