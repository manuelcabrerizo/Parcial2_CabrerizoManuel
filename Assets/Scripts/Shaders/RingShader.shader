Shader "Unlit/RingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "black" {}
		_ParticleColor ("Particle Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "QUEUE"="Transparent" }
        LOD 100

        Pass
        {
			Blend SrcAlpha One
			Cull Off
			ZWrite Off

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
			sampler2D _NoiseTex;
            float4 _MainTex_ST;
			float3 _ParticleColor;

			float Random(in float2 st)
			{
				return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
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
				float2 noiseUv = i.uv;
				noiseUv.y += _Time.x;
				float4 noise = tex2D(_NoiseTex, noiseUv*0.25f);

				float2 textureUv = i.uv + noise.xy;
				float4 col = tex2D(_MainTex, textureUv) * 2.0f;
				col.rgb *= float3(1.0f, 1.0f, 0.0f);
				
				float2 uv = i.uv;
				float2 particlesUv = floor(uv * 256);
				if(Random(particlesUv) >= 0.99)
				{
					col.rgb = _ParticleColor; 
				}
				col.w *= 0.5f;
				return col;
            }

            ENDCG
        }
    }
}
