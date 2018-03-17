using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVGPainterUnity{
	public enum PainterAnimationState {
		None,
		Forward,
		Rewind,
		Complete
	}

	public class Painter : MonoBehaviour {
		public List<Vector3> originalPoints = new List<Vector3> ();
		public LineRenderer line;
		public int sMaskValueID;
		public System.Func<float, float, float, float, float> easing = null;

		public float duration = 0f;
		public float delay = 0f;

		private float startTime = 0f;
		private PainterAnimationState state = PainterAnimationState.None;

		// Use this for initialization
		void Awake () {
			startTime = Time.time;
		}

		public void Play(float _delay = 0f, System.Func<float, float, float, float, float> _easing = null) {
			state = PainterAnimationState.Forward;
			SetUpParams (_delay, _easing);
		}

		public void Rewind(float _delay = 0f, System.Func<float, float, float, float, float> _easing = null) {
			state = PainterAnimationState.Rewind;
			SetUpParams (_delay, _easing);
		}

		private void SetUpParams(float _delay = 0f, System.Func<float, float, float, float, float> _easing = null) {
			delay = _delay;
			easing = _easing;
			startTime = Time.time;
			startTime += delay;
		}
			
		public void UpdateLine() {
			if (state == PainterAnimationState.None || state == PainterAnimationState.Complete) {
				return;
			}

			float end = Time.time;
			if(end < startTime) {
				return;
			}

			float elapsed = end - startTime;
			if (elapsed >= duration) {
				// complete
				if (state == PainterAnimationState.Forward) {
					SetMaskValue (1f);
				} else if (state == PainterAnimationState.Rewind) {
					SetMaskValue (0f);
				}

				state = PainterAnimationState.Complete;
			} else {
				// update
				float changeVal = 0f;
				if (state == PainterAnimationState.Forward) {
					if (easing != null) {
						changeVal = easing (elapsed, 0f, 1f, duration);
					} else {
						changeVal = (elapsed / duration * 1.0f);
					}
				} else if (state == PainterAnimationState.Rewind) {
					if (easing != null) {
						changeVal = 1.0f-easing (elapsed, 0f, 1f, duration);
					} else {
						changeVal = 1.0f-(elapsed / duration * 1.0f);
					}
				}

				SetMaskValue (changeVal);
			}
		}

		public void SetMaskValue(float val) {
			line.material.SetFloat (sMaskValueID, val);
		}

		public PainterAnimationState GetState() {
			return state;
		}
	}
}
