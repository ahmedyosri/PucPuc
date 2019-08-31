Shader "Unlit/UnlitAnimatedTextureShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
        _SpeedX ("SpeedX", float) = 0.0
        _SpeedY ("SpeedY", float) = 0.0
    }
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vertprepare
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
			float4 _Color;
			float _SpeedX;
			float _SpeedY;

            v2f vertprepare(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float2 newUv = i.uv;
				newUv.x += _Time * _SpeedX;
				newUv.y += _Time * _SpeedY;
                float4 col = tex2D(_MainTex, newUv);
				col.a = col.r;
				col.rgb = _Color.rgb;
                return col;
            }
            ENDCG
        }
    }
}
