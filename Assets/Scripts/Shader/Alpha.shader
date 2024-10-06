Shader "Unlit/Alpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NDITex("NDI Texture", 2D) = "white" {}
       [MaterialToggle] _MetaBallMode("Meta Ball Mode", Float) = 0
        _NumRobot("num robot", Int) = 1
        _UserPosition("user position", Vector) = (0.0,0.0, 0.0, 0.0)
        _RobotPos_1("robot position 1", Vector) = (0.0,0.0, 0.0, 0.0)
        _RobotPos_2("robot position 2", Vector) = (0.0,0.0, 0.0, 0.0)
        _RobotPos_3("robot position 3", Vector) = (0.0,0.0, 0.0, 0.0)
        _MetaValue("meta meta value", Float) = 0.55
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
             ZWrite OFF
             Ztest LEqual

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NDITex;
            float4 _MainTex_ST;
            float   _MetaBallMode;
            float _MetaValue;

            int _NumRobot;

            Vector _UserPosition;
            Vector _RobotPos_1;
            Vector _RobotPos_2;
            Vector _RobotPos_3;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_MetaBallMode)
                {
                    
                    float2  p = -(i.uv.xy * 2.0 - 1.0) * 2.0;
                    float2 upos = float2(_UserPosition.x, _UserPosition.z) * 0.313;
                    float2 r1pos = float2(_RobotPos_1.x, _RobotPos_1.z) * 0.313;

                    float c = 0;
                    c += (0.08 - _MetaValue) / length(p - upos);
                    c += _MetaValue / length(p - r1pos);
                   // c += 0.05 / length(p - float2(1.0, 1.0));

                    c = step(0.5, c);

                   float  a = (c > 0.0) ? 0.0 : 1.0;
                    fixed4 col = fixed4(c, c, c, a);
                    return col;
                }
                else
                {
                    fixed4 col = tex2D(_NDITex, i.uv);
                    fixed4 whiteColor = fixed4(1.0, 1.0, 1.0, 0.0);

                    //col.a = 1.0 - col.a;

                    return col;
                 }
            }
            ENDCG
        }
    }
}
