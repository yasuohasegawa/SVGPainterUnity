using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVGPainterUnity{
	public enum PainterState {
		None,
		Animating,
		Complete
	}

	public class SVGPainter : MonoBehaviour {
		private List<Painter> painters = new List<Painter>();

		private PainterState state = PainterState.None;

		private ToPoints toPoints;

		private System.Action onComplete = null;
		private System.Action onRewindComplete = null;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if (state == PainterState.Complete) {
				return;
			}

			if(painters.Count >= 1){
				int checkCompleteCount = 0;
				for (int i = 0; i < painters.Count; i++) {
					painters [i].UpdateLine ();
					PainterAnimationState pstate = painters [i].GetState ();
					if(pstate == PainterAnimationState.Complete){
						checkCompleteCount++;
					}
				}
				if(checkCompleteCount>=painters.Count){
					state = PainterState.Complete;
					if (onComplete != null) {
						onComplete ();
						onComplete = null;
					}

					if(onRewindComplete != null){
						onRewindComplete ();
						onRewindComplete = null;
					}
				}
			}
		}

		public void Init(string file, float w = 0.002f, Color col = default(Color), bool isAnimate = true, bool isCanvas = false) {
			SVGDataParser parser = gameObject.AddComponent<SVGDataParser> ();
			List<string> paths = parser.Load (file);

			if (col.Equals(new Color(0f,0f,0f,0f))) {
				col = new Color (1f, 1f, 1f, 1f);
			}

			toPoints = new ToPoints ();

			for(int i = 0; i<paths.Count; i++){
				GameObject path = null;

				if (!isCanvas) {
					path = CreatePath (paths [i], w, col, parser.GetSize ().x, parser.GetSize ().y);
				} else {
					path = CreateCanvasPath (paths [i], w, col, parser.GetSize ().x, parser.GetSize ().y);
				}

				Painter painter = path.GetComponent<Painter> ();
				painters.Add (painter);
				if (!isAnimate) {
					painter.SetMaskValue(1f);
				}
			}
		}

		public void InitCanvas(string file, float w = 1f, Color col = default(Color), bool isAnimate = true){
			Init (file, w, col, isAnimate, true);
		}

		public void Play(float duration = 3f, System.Func<float, float, float, float, float> _easing = null, System.Action callback = null) {
			state = PainterState.Animating;
			onComplete = callback;
			for (int i = 0; i < painters.Count; i++) {
				painters [i].duration = duration;
				painters [i].Play (0f, _easing);
			}
		}
			
		public void Rewind(float duration = 3f, System.Func<float, float, float, float, float> _easing = null, System.Action callback = null) {
			state = PainterState.Animating;
			onRewindComplete = callback;
			for (int i = 0; i < painters.Count; i++) {
				painters [i].duration = duration;
				painters [i].Rewind (0f, _easing);
			}
		}

		// for 3d space
		private GameObject CreatePath(string path, float w, Color col, float width, float height) {
			GameObject pathObj = new GameObject ("path");
			pathObj.transform.parent = gameObject.transform;
			pathObj.transform.localPosition = Vector3.zero;
			pathObj.transform.localScale = new Vector3(1f,1f,1f);

			LineRenderer l = pathObj.AddComponent<LineRenderer> ();
			Painter painter = pathObj.AddComponent<Painter> ();
			PathData data = toPoints.GetPointsFromPath (path);

			Vector3 eulerAngles = new Vector3 (0f, 180f, 180f);
			Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
			Matrix4x4 m = Matrix4x4.Rotate(rotation);

			Camera c = Camera.main;
			for (int i = 0; i < data.points.Count; i++) {
				Vector3 p = c.ScreenToWorldPoint (new Vector3 (data.points [i].x+((Screen.width-width)*0.5f), data.points [i].y+((Screen.height-height)*0.5f), c.nearClipPlane));
				p.y -= c.transform.localPosition.y*2f;

				p = m.MultiplyPoint3x4(p);
				p.z = 0f;
				data.points [i] = p;
			}

			l.positionCount = data.points.Count;
			l.SetPositions (data.points.ToArray());
			l.startWidth = w;
			l.endWidth = w;
			l.useWorldSpace = false;
			l.startColor = col;
			l.endColor = col;
			l.material = new Material(Shader.Find("Custom/SVGLine"));

			painter.originalPoints = data.points;
			painter.lineMat = l.material;
			painter.sMaskValueID = Shader.PropertyToID("_SVGLineMaskValue");
			return pathObj;
		}

		// for 2d canvas
		private GameObject CreateCanvasPath(string path, float w, Color col, float width, float height) {
			GameObject pathObj = new GameObject ("path");
			pathObj.transform.parent = gameObject.transform;
			pathObj.transform.localPosition = Vector3.zero;
			pathObj.transform.localScale = new Vector3(1f,1f,1f);

			CanvasLineRenderer cvs = pathObj.AddComponent<CanvasLineRenderer> ();
			Painter painter = pathObj.AddComponent<Painter> ();
			PathData data = toPoints.GetPointsFromPath (path);

			Vector3 eulerAngles = new Vector3 (0f, 180f, 180f);
			Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
			Matrix4x4 mRot = Matrix4x4.Rotate(rotation);
			//Matrix4x4 mSc = Matrix4x4.Scale(new Vector3(0.1f,0.1f,0.1f));

			Camera c = Camera.main;
			for (int i = 0; i < data.points.Count; i++) {
				Vector3 p = new Vector3 (data.points [i].x - (width * 0.5f), data.points [i].y - (height * 0.5f), 0f);
				p = mRot.MultiplyPoint3x4(p);
				//p = mSc.MultiplyPoint3x4(p);
				p.z = 0f;
				data.points [i] = p;
			}

			Material mat = new Material(Shader.Find("Custom/SVGLine"));
			cvs.UpdateLine(data.points,w,col,mat);

			painter.originalPoints = data.points;
			painter.lineMat = mat;
			painter.sMaskValueID = Shader.PropertyToID("_SVGLineMaskValue");

			return pathObj;
		}

	}
}
