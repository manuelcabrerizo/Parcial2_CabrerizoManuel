Shader "Custom/FeebackShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1)
		_Tint ("Tint", Color) = (0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 wPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float3 _Color;
			float3 _Tint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				float3 n = normalize(i.normal);
				float3 l = normalize(UnityWorldSpaceLightDir(i.wPos));
				float3 v = normalize(_WorldSpaceCameraPos - i.wPos);

				float diffuse = max(dot(n, l), 0.2f);
				float fresnel = saturate(dot(n, v));

                float4 col = tex2D(_MainTex, i.uv);
				col.xyz *= 3.0f;
				col.xyz *= _Color;
				col.xyz *= diffuse;
				col.xyz += _Tint * (1.0f - fresnel);

                return col;
            }
            ENDCG
        }
    }
}
