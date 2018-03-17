using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Text.RegularExpressions;
using System.Linq;

namespace SVGPainterUnity{
	public class SVGDataParser: MonoBehaviour {

		private List<string> paths = new List<string>();
		private Vector2 size = Vector2.zero;

		void Start() {
			
		}

		public List<string> Load(string path) {
			TextAsset xmlTextAsset = Instantiate(Resources.Load(path)) as TextAsset;
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlTextAsset.text);

			XmlNodeList nodes = xmlDoc.GetElementsByTagName("path");

			string viewBox = xmlDoc.GetElementsByTagName ("svg") [0].Attributes.GetNamedItem ("viewBox").Value;
			Debug.Log (xmlDoc.GetElementsByTagName("svg")[0].Attributes.GetNamedItem ("viewBox").Value);
			List<string> res = Regex.Split (viewBox, @"[ ]").ToList ();
			size.x = float.Parse(res [2]);
			size.y = float.Parse(res [3]);

			foreach (XmlNode node in nodes) {
				string p = node.Attributes.GetNamedItem ("d").Value;
				paths.Add (p);
			}

			return paths;
		}

		public Vector2 GetSize() {
			return size;
		}

		public List<string> GetPaths() {
			return paths;
		}
	}
}