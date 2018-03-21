using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// from http://wordpress.notargs.com/blog/blog/2015/08/30/unityuguiで曲線を描画する/
// edited for using "setMesh" method
namespace SVGPainterUnity {
	public class CanvasLineRenderer : MonoBehaviour {

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void UpdateLine(List<Vector3> points, float width, Color color, Material material) {
			var canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
			if(canvasRenderer == null){
				canvasRenderer = gameObject.AddComponent<CanvasRenderer>();
			}

			List<Vector3> vertices = new List<Vector3> ();
			List<Color> faceColors = new List<Color> ();
			List<int> indices = new List<int> ();
			List<Vector2> uvs = new List<Vector2> ();

			Mesh mesh = new Mesh ();

			float startUvX = 0f;
			float endUvX = 0f;
			for (int i = 0; i < points.Count - 1; i++) {
				Vector3 p0 = points [i + 0];
				Vector3 p1 = points [i + 1];

				Vector2? point0;
				Vector2 point1 = new Vector2(p0.x,p0.y);
				Vector2 point2 = new Vector2(p1.x,p1.y);
				Vector2? point3;

				if (i - 1 < 0) point0 = null;
				else point0 = points[i - 1];

				if (i + 2 > points.Count - 1) point3 = null;
				else point3 = points[i + 2];

				var normal1 = CalcNormal(point0, point1, point2);
				var normal2 = CalcNormal(point1, point2, point3);

				Vector2 vert0 = point1 - normal1 * width;
				Vector2 vert1 = point2 - normal2 * width;
				Vector2 vert2 = point2 + normal2 * width;
				Vector2 vert3 = point1 + normal1 * width;

				vertices.Add(new Vector3(vert0.x,vert0.y,0f));
				vertices.Add(new Vector3(vert1.x,vert1.y,0f));
				vertices.Add(new Vector3(vert2.x,vert2.y,0f));
				vertices.Add(new Vector3(vert3.x,vert3.y,0f));

				faceColors.Add(new Color (color.r, color.g, color.b));
				faceColors.Add(new Color (color.r, color.g, color.b));
				faceColors.Add(new Color (color.r, color.g, color.b));
				faceColors.Add(new Color (color.r, color.g, color.b));

				indices.Add(i * 4  ); //1
				indices.Add(i * 4 + 1 ); //2
				indices.Add(i * 4 + 2 ); //3
				indices.Add(i * 4  ); //1
				indices.Add(i * 4 + 2 ); //3
				indices.Add(i * 4 + 3 ); //4

				endUvX = (float)i/(float)(points.Count - 2)*1.0f;
				uvs.Add(new Vector2(startUvX, 0f));
				uvs.Add(new Vector2(endUvX, 0f));
				uvs.Add(new Vector2(endUvX, 1f));
				uvs.Add(new Vector2(startUvX, 1f));
				startUvX = endUvX;
			}

			mesh.vertices = vertices.ToArray ();
			mesh.triangles = indices.ToArray ();
			mesh.uv = uvs.ToArray ();
			mesh.colors = faceColors.ToArray ();

			canvasRenderer.SetMesh(mesh);
			canvasRenderer.SetMaterial(material, Texture2D.whiteTexture);
		}
			
		private Vector2 CalcNormal(Vector2? prev, Vector2 current, Vector2? next) {
			var dir = Vector2.zero;
			if (prev.HasValue) dir += prev.Value - current;
			if (next.HasValue) dir += current - next.Value;
			dir = new Vector2(-dir.y, dir.x).normalized;
			return dir;
		}
	}
}