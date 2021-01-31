Shader "Unlit/Unlit-Rad"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Position("Position", Vector) = (0,0,0)
        _Radius("Radius", Range(0,100) ) = 0
        _Softness("Softness", Range(0,100) ) = 0
    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
		//Zwrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            float4 _Position;
            half _Radius;
            half _Softness;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                half d = length(max(abs(i.uv - 0.5),_Radius) - _Radius);
                if( d > _Softness) discard;             
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);               
                return col;
            }
            ENDCG
        }
    }
}
