// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SVGLine"
{
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_TintColor ("TintColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_SVGLineMaskValue("MaskValue",float) = 0.0
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _TintColor;
		half4 _MainTex_ST;
						
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			fixed4 vertexColor : COLOR;
		};

		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = UnityObjectToClipPos (v.vertex);	
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.vertexColor = v.color * _TintColor;
					
			return o; 
		}

		float _SVGLineMaskValue;

		fixed4 frag( v2f i ) : COLOR {
			if(_SVGLineMaskValue <= 0.0){
				discard;
			}	

			if(i.uv.x>=_SVGLineMaskValue){
				discard;
			}
			return tex2D (_MainTex, i.uv.xy) * i.vertexColor;
		}
	
	ENDCG
	
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One One
		
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
				
	} 
	FallBack Off
}