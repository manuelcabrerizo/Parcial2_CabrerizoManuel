Shader "Unlit/FadeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Fading ("Fading", Float) = 1.0
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Fading;

			float Random(in float2 st)
			{
				return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
			}

			float2 ClosestPointToRect(float2 p, float2 min, float2 max)
			{
				float x = clamp(p.x, min.x, max.x);
				float y = clamp(p.y, min.y, max.y);
				return float2(x, y);
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

				float2 pointA = i.uv;				
				float2 pointB = ClosestPointToRect(pointA, float2(0.2f, 0.2f), float2(0.8f, 0.8f));

				float diff = length(pointA - pointB);
				diff = max(diff, 0.0f);

				float2 uv = floor(i.uv * 256.0f);				
				if(Random(uv) > 0.5f)
				{
					diff *= 0.0f;
				}

				if(Random(uv) >= _Fading)
				{
					col.w = _Fading + diff;
				}
				return col;
            }

            ENDCG
        }
    }
}
