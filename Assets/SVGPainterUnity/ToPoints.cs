using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

// https://github.com/colinmeinke/svg-points/blob/master/src/toPoints.js

namespace SVGPainterUnity{
	public class CurveData {
		public string type;
		public float x1;
		public float y1;
		public float x2;
		public float y2;
		public float rx;
		public float ry;
		public float xAxisRotation;
		public int largeArcFlag;
		public int sweepFlag;
	}

	public class PointData {
		public Vector3 pos;
		public CurveData curve;
		public bool moveTo = false;

		public PointData(Vector3 _pos, CurveData _curve, bool _moveTo = false) {
			pos = _pos;
			curve = _curve;
			moveTo = _moveTo;
		}
	}

	public class PathData {
		public List<PointData> svgPoints = new List<PointData> ();
		public List<Vector3> points = new List<Vector3> ();
	}

	public class ToPoints {
		private string validCommands = @"[MmLlHhVvCcSsQqTtAaZz]";
		private List<string> relativeCommands = new List<string>{"a","c","h","l","m","q","s","t","v"};
		private Dictionary<string, int> commandLengths = new Dictionary<string, int>(){
			{"A",7},{"C",6},{"H",1},{"L",2},{"M",2},{"Q",4},{"S",4},{"T",2},{"V",1},{"Z",0}
		};

		//private List<string>  optionalArcKeys = new List<string>{"xAxisRotation","largeArcFlag","sweepFlag"};

		public ToPoints() {
			
		}

		public bool IsRelative(string command) {
			return relativeCommands.IndexOf(command) != -1;
		}

		public List<string> GetCommands(string str) {
			return Regex.Matches(str,validCommands).Cast<Match>().Select(m => m.Groups[0].Value).ToList();
		}

		public List<List<float>> GetParams(string str) {
			string pattern1 = @"[0-9]+-";
			string pattern2 = @"\.[0-9]+";
			string pattern3 = @"[ ,]+";
			List<string> res = Regex.Split (str, validCommands).ToList ();
			res = map ((string val)=>{ 
				if(Regex.IsMatch(val,pattern1)){
					MatchCollection mc = Regex.Matches(val,pattern1);
					foreach (Match item in mc) {
						val = val.Replace(item.Value,item.Value.Substring (0, item.Value.Length-1) + " -");
					}
				}
				return val;
			}, res);
			res = map ((string val) => {
				if(Regex.IsMatch(val,pattern2)){
					MatchCollection mc = Regex.Matches(val,pattern2);
					foreach (Match item in mc) {
						val = val.Replace(item.Value,item.Value + " ");
					}
				}
				return val;
			}, res);
			res = map ((string val) => {
				return val.Trim();
			}, res).Where(v => v.Length > 0).ToList();

			List<List<float>> res2 = new List<List<float>> ();
			for(int i = 0; i<res.Count; i++) {
				res2.Add(Regex.Split (res[i], pattern3).Select(float.Parse).Where(v => !float.IsNaN(v)).ToList());
			}

			/*
			for (int i = 0; i < res2.Count; i++) {
				for (int j = 0; j < res2[i].Count; j++) {
					Debug.Log (res2[i][j]);
				}
			}
			*/
			return res2;
		}

		public PathData GetPointsFromPath(string d) {
			List<string> commands = GetCommands (d);
			List<List<float>> _params = GetParams (d);
			List<PointData> points = new List<PointData> ();
			List<Vector3> pathPoints = new List<Vector3> ();

			Vector3 moveTo = Vector3.zero;
			CurveData curve = null;
			Vector2 diff = Vector2.zero;
			float x, y, x1, y1, x2, y2;
			for (int i = 0, l = commands.Count; i < l; i++) {
				string command = commands [i];
				string upperCaseCommand = command.ToUpper ();
				int commandLength = commandLengths [upperCaseCommand];
				bool relative = IsRelative (command);

				if (commandLength > 0) {
					List<float> commandParams = _params [0];
					_params.RemoveAt (0);

					int iterations = commandParams.Count / commandLength;

					for (int j = 0; j < iterations; j++) {
						PointData prevPoint = (points.Count != 0) ? points [points.Count - 1] : new PointData (Vector3.zero, null);

						switch (upperCaseCommand) {
						case "M":
							x = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							if (j == 0) {
								moveTo = new Vector3 (x, y, 0f);
								points.Add (new PointData (moveTo, null, true));

								pathPoints.Add (moveTo);
							} else {
								Vector3 mpoint = new Vector3 (x, y, 0f);
								points.Add (new PointData (mpoint, null));
		
								pathPoints.Add (mpoint);
							}
							break;

						case "L":
							x = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							Vector3 lpoint = new Vector3 (x, y, 0f);
							points.Add (new PointData (lpoint, null));

							pathPoints.Add (lpoint);
							break;

						case "H":
							x = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							Vector3 hpoint = new Vector3 (x, prevPoint.pos.y, 0f);
							points.Add (new PointData (hpoint, null));

							pathPoints.Add (hpoint);
							break;

						case "V":
							y = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							Vector3 vpoint = new Vector3 (prevPoint.pos.x, y, 0f);
							points.Add (new PointData (vpoint, null));

							pathPoints.Add (vpoint);
							break;

						case "A":
							// TODO: try to convert arc to points
							curve = new CurveData ();
							curve.type = "arc";
							curve.rx = commandParams [0];
							commandParams.RemoveAt (0);
							curve.ry = commandParams [0];
							commandParams.RemoveAt (0);
							curve.xAxisRotation = commandParams [0];
							commandParams.RemoveAt (0);
							curve.largeArcFlag = (int)commandParams [0];
							commandParams.RemoveAt (0);
							curve.sweepFlag = (int)commandParams [0];
							commandParams.RemoveAt (0);

							x = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							// 最後全体を、xAxisRotationする。
							//Debug.Log (">>>>> prev:"+new Vector3 (prevPoint.pos.x, prevPoint.pos.y, 0f));
							//Debug.Log (">>>>> current:"+new Vector3 (x, y, 0f));

							points.Add (new PointData (new Vector3 (x, y, 0f), curve));

								/*
								for (let k of optionalArcKeys) {
									if (points[ points.length - 1 ][ 'curve' ][ k ] === 0) {
										delete points[ points.length - 1 ][ 'curve' ][ k ]
									}
								}
								*/

							break;
						case "C":
							x1 = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y1 = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							x2 = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y2 = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							curve = new CurveData ();
							curve.type = "cubic";
							curve.x1 = x1;
							curve.y1 = y1;
							curve.x2 = x2;
							curve.y2 = y2;

							x = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							points.Add (new PointData (new Vector3 (x, y, 0f), curve));


							getCurve (ref pathPoints, prevPoint.pos.x, prevPoint.pos.y, x1, y1, x2, y2, x, y, "cubic");

							break;
						case "S":
							float sx2 = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							float sy2 = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							float sx = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							float sy = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							diff = Vector2.zero;

							float sx1;
							float sy1;
							if (prevPoint.curve != null && prevPoint.curve.type == "cubic") {
								diff.x = Mathf.Abs (prevPoint.pos.x - prevPoint.curve.x2);
								diff.y = Mathf.Abs (prevPoint.pos.y - prevPoint.curve.y2);
								sx1 = prevPoint.pos.x < prevPoint.curve.x2 ? prevPoint.pos.x - diff.x : prevPoint.pos.x + diff.x;
								sy1 = prevPoint.pos.y < prevPoint.curve.y2 ? prevPoint.pos.y - diff.y : prevPoint.pos.y + diff.y;
							} else {
								diff.x = Mathf.Abs (sx - sx2);
								diff.y = Mathf.Abs (sy - sy2);
								sx1 = prevPoint.pos.x;
								sy1 = prevPoint.pos.y;
							}

							curve = new CurveData ();
							curve.type = "cubic";
							curve.x1 = sx1;
							curve.y1 = sy1;
							curve.x2 = sx2;
							curve.y2 = sy2;

							points.Add (new PointData (new Vector3 (sx, sy, 0f), curve));

							getCurve (ref pathPoints, prevPoint.pos.x, prevPoint.pos.y, sx1, sy1, sx2, sy2, sx, sy, "cubic");
							break;
						case "Q":
							x1 = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y1 = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							curve = new CurveData ();
							curve.type = "quadratic";
							curve.x1 = x1;
							curve.y1 = y1;

							x = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							y = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							points.Add (new PointData (new Vector3 (x, y, 0f), curve));

							getCurve (ref pathPoints, prevPoint.pos.x, prevPoint.pos.y, x1, y1, 0f, 0f, x, y, "quadratic");
							break;
						case "T":
							float tx = (relative ? prevPoint.pos.x : 0f) + commandParams [0];
							commandParams.RemoveAt (0);
							float ty = (relative ? prevPoint.pos.y : 0f) + commandParams [0];
							commandParams.RemoveAt (0);

							float tx1;
							float ty1;

							if (prevPoint.curve != null && prevPoint.curve.type == "quadratic") {
								diff = new Vector2 (
									Mathf.Abs (prevPoint.pos.x - prevPoint.curve.x1),
									Mathf.Abs (prevPoint.pos.y - prevPoint.curve.y1)
								);

								tx1 = prevPoint.pos.x < prevPoint.curve.x1 ? prevPoint.pos.x - diff.x : prevPoint.pos.x + diff.x;
								ty1 = prevPoint.pos.y < prevPoint.curve.y1 ? prevPoint.pos.y - diff.y : prevPoint.pos.y + diff.y;
							} else {
								tx1 = prevPoint.pos.x;
								ty1 = prevPoint.pos.y;
							}

							curve = new CurveData ();
							curve.type = "quadratic";
							curve.x1 = tx1;
							curve.y1 = ty1;

							points.Add (new PointData (new Vector3 (tx, ty, 0f), curve));

							getCurve (ref pathPoints, prevPoint.pos.x, prevPoint.pos.y, tx1, ty1, 0f, 0f, tx, ty, "quadratic");

							break;
						}
					}
				} else {
					PointData prevPoint = (points.Count != 0) ? points [points.Count - 1] : new PointData (Vector3.zero, null);
					if (prevPoint.pos.x != moveTo.x || prevPoint.pos.y != moveTo.y) {
						Vector3 moveToPoint = new Vector3 (moveTo.x, moveTo.y, 0f);
						points.Add (new PointData (moveToPoint, null));
						pathPoints.Add (moveToPoint);
					}
				}
			}

			PathData pdata = new PathData ();
			pdata.svgPoints = points;
			pdata.points = pathPoints;

			return pdata;
		}

		private void getCurve(ref List<Vector3> curve, float startX, float startY, float x1, float y1, float x2 = 0f, float y2 = 0f, 
			float endX = 0f, float endY = 0f, string type = "cubic",float _accuracy = 0f){

			float accuracy = 0.1f; // 0.033 0.066 0.1
			if(_accuracy != 0f){
				accuracy = _accuracy;
			}

			Vector3 p = Vector3.zero;
			if(type == "cubic"){
				for (float i = 0; i < 1f; i += accuracy ){
					p = interpolateCubicBezier(new Vector3(startX,startY,0f),new Vector3(x1,y1,0f),new Vector3(x2,y2,0f),
						new Vector3(endX,endY,0f),i);
					curve.Add(p);
				}

				p = interpolateCubicBezier (new Vector3 (startX, startY, 0f), new Vector3 (x1, y1, 0f), new Vector3 (x2, y2, 0f),
					new Vector3 (endX, endY, 0f), 1f);
				curve.Add(p);
			} else if(type == "quadratic"){
				for (float i = 0; i < 1f; i += accuracy ){
					p = interpolateQuadraticBezier(new Vector3(startX,startY,0f),new Vector3(x1,y1,0f),
						new Vector3(endX,endY,0f),i);
					curve.Add(p);
				}

				p = interpolateQuadraticBezier (new Vector3 (startX, startY, 0f), new Vector3 (x1, y1, 0f),
					new Vector3 (endX, endY, 0f), 1f);
				curve.Add(p);
			}
		}

		public Vector3 interpolateQuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t) {
			return new Vector3( 
				(Mathf.Pow(1f - t, 2f) * start.x) +
					(2f * (1f - t) * t * control.x) +
				(Mathf.Pow(t, 2f) * end.x),
				(Mathf.Pow(1f - t, 2f) * start.y) +
					(2f * (1f - t) * t * control.y) +
				(Mathf.Pow(t, 2f) * end.y)
			,0f);
		}

		public Vector3 interpolateCubicBezier(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t) {
			return new Vector3( 
					(Mathf.Pow(1f - t, 3f) * start.x) +
				(3f * Mathf.Pow(1f - t, 2f) * t * control1.x) +
				(3f * (1f - t) * Mathf.Pow(t, 2f) * control2.x) +
				(Mathf.Pow(t, 3f) * end.x),(Mathf.Pow(1f - t, 3f) * start.y) +
				(3f * Mathf.Pow(1f - t, 2f) * t * control1.y) +
				(3f * (1f - t) * Mathf.Pow(t, 2f) * control2.y) +
				(Mathf.Pow(t, 3f) * end.y),0f);
		}

		public List<T> map<T> (System.Func<T, T> f, List<T> list) {
			List<T> res = new List<T> ();
			foreach (var num in list) {
				res.Add (f (num));
			}
			return res;
		}

	}
}