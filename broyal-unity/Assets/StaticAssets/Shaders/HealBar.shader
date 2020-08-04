Shader "Unlit/HealBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fill ("Fill", float) = 0
        _ColourDamaged("ColourDamaged", Color) = (1,0,0,1)
        _ColourHealthy("ColourHealthy", Color) = (0,1,0,1)
        _ScaleX ("Scale X", Float) = 1.0
        _ScaleY ("Scale Y", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }
        LOD 100

        Pass
        {
            ZTest Off
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };           
            
          
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColourDamaged;
            float4 _ColourHealthy;
            uniform float _ScaleX;
            uniform float _ScaleY;           
            
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
            UNITY_DEFINE_INSTANCED_PROP(float, _Fill)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                //float fill = UNITY_ACCESS_INSTANCED_PROP(Props, _Fill);
                //o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.vertex = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
                               + float4(v.vertex.x, v.vertex.y, 0.0, 0.0)
                                             * float4(_ScaleX, _ScaleY, 1.0, 1.0));                

                o.uv = v.uv;

                //o.uv.x += 0.5 - fill;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return tex2D(_MainTex, float2(i.uv.xy));   
                UNITY_SETUP_INSTANCE_ID(i);
                float fill = UNITY_ACCESS_INSTANCED_PROP(Props, _Fill);
                
                float4 color = lerp(_ColourHealthy * 3.0f, _ColourDamaged, smoothstep(fill - 0.06, fill + 0.06, i.uv.x));                               
                return color;
            }
            ENDCG
        }
    }
}
