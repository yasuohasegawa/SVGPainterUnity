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

		public void Init(string file, float w = 0.002f, bool isAnimate = true) {
			SVGDataParser parser = gameObject.AddComponent<SVGDataParser> ();
			List<string> paths = parser.Load (file);

			toPoints = new ToPoints ();

			for(int i = 0; i<paths.Count; i++){
				GameObject path = CreatePath (paths[i],w,parser.GetSize().x,parser.GetSize().y);
				Painter painter = path.GetComponent<Painter> ();
				painters.Add (painter);
				if (!isAnimate) {
					painter.SetMaskValue(1f);
				}
			}
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

		private GameObject CreatePath(string path, float w, float width, float height) {
			GameObject pathObj = new GameObject ("path");
			pathObj.transform.parent = gameObject.transform;

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
			l.material = new Material(Shader.Find("Custom/SVGLine"));

			painter.originalPoints = data.points;
			painter.line = l;
			painter.sMaskValueID = Shader.PropertyToID("_SVGLineMaskValue");
			return pathObj;
		}
	}
}
