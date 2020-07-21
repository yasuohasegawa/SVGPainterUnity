SVG Line Painter for Unity
=================
![Screenshot](screen0.png)

It works like a js version of SVG line animation like "LazyLinePainter" which you can find js library link in below.
https://github.com/camoconnell/lazy-line-painter

Also SVG part of code is a port of https://github.com/colinmeinke/svg-points/blob/master/src/toPoints.js


**Preparing your SVG data** <br>
Create your Line art in your vector editor of choice
- Ensure there are no fills.
- No closed paths. i.e - Line needs a start and end.
- No compound path. If you have compound paths, you need to release.
- Crop Artboard nice & tight!
- If you draw simple circle, you need to add anchor point. Othewise library will not draw circle.
Export as .SVG (Default export options are fine)<br>
Convert SVG file to text file. e.g - You can rename like "test.svg.txt".<br>

![Screenshot](screen2.jpg)
<br>
![Screenshot](screen3.png)

## SetUp
Copy `Assets/SVGPainterUnity` directory to your Assets directory.<br>
Add a `SVGPainter` component to a GameObject.<br>
Add your SVG text file to `Resources/` directory.

![Screenshot](screen1.png)

## Example Code
![Screenshot](anim.gif)

Here is how to animate.

```C#
var svgPainter = GetComponent<SVGPainter> ();
svgPainter.Init ("test.svg");
svgPainter.Play (3f, PainterEasing.EaseInOutCubic);
```

If you want to rewind animation, here it is.

```C#
svgPainter.Play (3f, PainterEasing.EaseInOutCubic, () => {
	svgPainter.Rewind (3f, PainterEasing.EaseInOutCubic);
});
```

Setup line width and color.

```C#
svgPainter.Init ("test.svg",0.002f,new Color (1f, 1f, 1f));
```

## Canvas Example Code

```C#
var svgPainter = GetComponent<SVGPainter> ();
svgPainter.InitCanvas ("test.svg");
svgPainter.Play (3f, PainterEasing.EaseInOutCubic);
```

Setup line width and color.
```C#
svgPainter.InitCanvas ("test.svg",1f,new Color (1f, 1f, 1f));
```

## Compatibility
SVGLinePainter is only tested on MacOS with OpenGLCore, iOS13.4, Android10 and not sure about another platform.<br>
Editor:2019.4 LTS

## License
MIT
