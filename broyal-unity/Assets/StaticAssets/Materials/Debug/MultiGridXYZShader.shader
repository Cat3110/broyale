Shader "Unlit/MultiGridXYZ"
{
    Properties
    {
		_Color("Color", Color) = (1, 1, 1, 1)

		_LineColor1("LineColor1", Color) = (1, 1, 1, 0.5)
		_LineColor2("LineColor2", Color) = (1, 1, 1, 0.5)
		_LineColor3("LineColor3", Color) = (1, 1, 1, 0.5)
		_LineColor4("LineColor4", Color) = (1, 1, 1, 0.5)
		_LineColor5("LineColor5", Color) = (1, 1, 1, 0.5)

		_Grid1("Grid1", Vector) = (100,100,100,1)
		_Grid2("Grid2", Vector) = (100,100,100,1)
		_Grid3("Grid3", Vector) = (100,100,100,1)
		_Grid4("Grid4", Vector) = (100,100,100,1)
		_Grid5("Grid5", Vector) = (100,100,100,1)

		_FogColor("FogColor", Color) = (1, 1, 1, 1)
		//_FogDensity("FogDensity", float) = 0.001
		_Fog1("Fog1", Vector) = (1000,12000,0,0)
		_Fog2("Fog2", Vector) = (1000,12000,0,0)
		_Fog3("Fog3", Vector) = (1000,12000,0,0)
		_Fog4("Fog4", Vector) = (1000,12000,0,0)
		_Fog5("Fog5", Vector) = (1000,12000,0,0)

		[HDR] _EmissionColor1("Emission Color1", Color) = (0,0,0)
		[HDR] _EmissionColor2("Emission Color2", Color) = (0,0,0)
		[HDR] _EmissionColor3("Emission Color3", Color) = (0,0,0)
		[HDR] _EmissionColor4("Emission Color4", Color) = (0,0,0)
		[HDR] _EmissionColor5("Emission Color5", Color) = (0,0,0)
    }

    SubShader
    { 
		Tags {			
			"RenderType" = "Opaque"
			"Queue" = "Geometry-1"
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#define LINFOG(dist,start,end) saturate((dist * (-1 / (end - start)) + (end / (end - start))))
	
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD2;
				float4 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _Thickness;			

			float4 _Grid1;
			float4 _Grid2;
			float4 _Grid3;
			float4 _Grid4;
			float4 _Grid5;

			float4 _Color;

			float4 _LineColor1;
			float4 _LineColor2;
			float4 _LineColor3;
			float4 _LineColor4;
			float4 _LineColor5;

			float _Offset;
			float _Distance;

			float4 _FogColor;
			float _FogDensity;
			float4 _EmissionColor1;		
			float4 _EmissionColor2;
			float4 _EmissionColor3;
			float4 _EmissionColor4;
			float4 _EmissionColor5;
			float4 _Fog1;
			float4 _Fog2;
			float4 _Fog3;
			float4 _Fog4;
			float4 _Fog5;

            v2f vert (appdata v)
            {
                v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);				
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldPos.w = o.vertex.z;
                return o;
            }

			float sFract(float x, float sm) {

				// Extra smoothing factor. "1" is the norm.
				const float sf = 1.;

				// The hardware "fwidth" is cheap, but you could take the expensive route and
				// calculate it by hand if more quality was required.
				float2 u = float2(x, fwidth(x)*sf*sm);

				// Ollj's original formula with a transcendental term omitted.
				u.x = frac(u.x);
				u += (1. - 2.*u)*step(u.y, u.x);
				return clamp(1. - u.x / u.y, 0., 1.); // Cos term ommitted.
			}

			float sFloor(float x) { return x - sFract(x, 1.); }

			float smoothFract(float x, float sf) {
				x = frac(x); return min(x, x*(1. - x)*sf);
			}
			
			float DrawGrid(float3 coord, float thick, float offset )
			{
				float3 fracCoord = abs(frac(coord - offset) - offset);
				float3 deltas = fwidth(coord);
				float3 thickness = deltas * thick;
				float3 grid = fracCoord / thickness;

				float gridMin = min(grid.x, min(grid.y, grid.z));
				float lineColor = 1.0f - min(gridMin, 1.0f);

				return lineColor;
			}

			float4 DrawFogedGrid(float3 coord, float4 grid, float offset, float dist, float2 fog)
			{
				return lerp(0, DrawGrid(coord, grid, offset), LINFOG(dist, fog.x, fog.y));
			}
			

			float4 frag (v2f i) : SV_Target
            {
				//float w = UNITY_Z_0_FAR_FROM_CLIPSPACE(i.worldPos.w);
				//exp fog
				//float fogFactor = (_FogDensity / 0.69314718056) * (w);
				//fogFactor = exp2(-fogFactor);
				float dist = length(_WorldSpaceCameraPos - i.worldPos);

				float4 col = _Color;				

			/*	col.rgb = lerp(_Color, _LineColor1 + _EmissionColor1, DrawGrid(i.worldPos / _Grid1, _Grid1.w, _LineColor1.a));
				col.rgb = lerp(col.rgb, _LineColor2 + _EmissionColor2, DrawGrid(i.worldPos / _Grid2, _Grid2.w, _LineColor2.a));
				col.rgb = lerp(col.rgb, _LineColor3 + _EmissionColor3, DrawGrid(i.worldPos / _Grid3, _Grid3.w, _LineColor3.a));
				col.rgb = lerp(col.rgb, _LineColor4 + _EmissionColor4, DrawGrid(i.worldPos / _Grid4, _Grid4.w, _LineColor4.a));
				col.rgb = lerp(col.rgb, _LineColor5 + _EmissionColor5, DrawGrid(i.worldPos / _Grid5, _Grid5.w, _LineColor5.a));*/

				col.rgb = lerp(_Color, _LineColor1 + _EmissionColor1, DrawFogedGrid(i.worldPos / _Grid1, _Grid1.w, _LineColor1.a, dist, _Fog1.xy));
				col.rgb = lerp(col.rgb, _LineColor2 + _EmissionColor2, DrawFogedGrid(i.worldPos / _Grid2, _Grid2.w, _LineColor2.a, dist, _Fog2.xy));
				col.rgb = lerp(col.rgb, _LineColor3 + _EmissionColor3, DrawFogedGrid(i.worldPos / _Grid3, _Grid3.w, _LineColor3.a, dist, _Fog3.xy));
				col.rgb = lerp(col.rgb, _LineColor4 + _EmissionColor4, DrawFogedGrid(i.worldPos / _Grid4, _Grid4.w, _LineColor4.a, dist, _Fog4.xy));
				col.rgb = lerp(col.rgb, _LineColor5 + _EmissionColor5, DrawFogedGrid(i.worldPos / _Grid5, _Grid5.w, _LineColor5.a, dist, _Fog5.xy));

				//col.rgb = lerp(_Color, _LineColor + _EmissionColor, DrawGrid(coord, _Thickness, _Offset));
				//col.rgb = lerp(_FogColor.rgb, col.rgb, saturate(fogFactor));			

				//float d = length(i.vLineCenter - i.worldPos.xy);
				//float w = 100.; //uLineWidth;
				//
				//if (d > w)
				//	col = 0;
				//else
				//	col *= pow(float((w - d) / w), 1.5);
				//return col;

				return col;
            }
            ENDCG
        }
    }
}
