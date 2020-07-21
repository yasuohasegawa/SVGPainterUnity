using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGPainterUnity;

public class CanvasExample : MonoBehaviour {
	private SVGPainter svgPainter;

	// Use this for initialization
	void Start () {
		svgPainter = GetComponent<SVGPainter> ();
		svgPainter.InitCanvas ("test.svg");

		/* // if you want to try rewind animation, here it is.
		svgPainter.Play (3f, PainterEasing.EaseInOutCubic, () => {

			Debug.Log("Complete");

			svgPainter.Rewind (3f, PainterEasing.EaseInOutCubic, () => {
				Debug.Log("Rewind Complete");
			});
		});
		*/
	}


	private void Play()
	{
		svgPainter.Play(3f, PainterEasing.EaseInOutCubic, () =>
		{
			Debug.Log("Play complete!");
		});
	}

	private void Rewind()
	{
		svgPainter.Rewind(3f, PainterEasing.EaseInOutCubic, () => {
			Debug.Log("rewind complete!");
		});
	}

	private void Stop()
	{
		svgPainter.Stop(true);
	}

	private void LoopAnimation()
	{
		svgPainter.Play(3f, PainterEasing.EaseInOutCubic, () => {
			svgPainter.Rewind(3f, PainterEasing.EaseInOutCubic, () => {
				LoopAnimation();
			});
		});
	}

	public void OnPlay()
	{
		Play();
	}

	public void OnRewind()
	{
		Rewind();
	}

	public void OnStop()
	{
		Stop();
	}

	public void OnLoop()
	{
		LoopAnimation();
	}
}
