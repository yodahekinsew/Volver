// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mobile/Additive Texture" {
Properties {
    // [PerRendererData] _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Blend SrcAlpha OneMinusDstAlpha
    Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
    
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }
    
    SubShader {
        Pass {
            // SetTexture [_MainTex] {
            //     constantColor[_Color]
            //     combine texture * constant
            // }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                // float4 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                // float4 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = o.vertex;
                o.texcoord = v.texcoord;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            fixed4 _Color;
            float _Transparency, _Offset;

            fixed4 frag (v2f i) : SV_Target
            {
                // float2 coord = 0.5 + 0.5 * i.uv.xy / i.uv.w;
                // fixed4 tex = tex2D(_GrabTexture, float2(coord.x, 1 - coord.y));
                // return fixed4(lerp(tex.rgb, _Color.rgb, _Color.a), 1);
                fixed4 val = _Color*tex2D(_MainTex, i.texcoord);
                return val;
            }
            ENDCG
        }
    }
}
}
