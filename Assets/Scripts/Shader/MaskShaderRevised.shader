Shader "Unlit/MaskShaderRevised"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NDITex("NDI Texture", 2D) = "white" {}
        _MaskChannel("Channel", Int) = 0
        _NumRobot("num robot", Int) = 1
        _UserPosition_0("user position", Vector) = (0.0,0.0, 0.0, 0.0)
        _RobotPos_0("robot position 1", Vector) = (0.0,0.0, 0.0, 0.0)
        _RobotPos_2("robot position 2", Vector) = (0.0,0.0, 0.0, 0.0)
        _RobotPos_3("robot position 3", Vector) = (0.0,0.0, 0.0, 0.0)
        _MetaValue("meta meta value", Float) = 0.55
        _MapScale("map scale", Float) = 2.535
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
            float   _MaskChannel;
            float _MetaValue;
            float _Disntance;
            float _MapScale;

            int _NumRobot;
            int _ActiveUserNum;
            
            static int _NUM_USER = 10;
            Vector _UserPositions[10];
            Vector _UserPosition_0;
            Vector _UserPosition_1;
            Vector _UserPosition_2;
            Vector _UserPosition_3;

            Vector _RobotPos_0;
            Vector _RobotPos_1;
            Vector _RobotPos_2;
            Vector _RobotPos_3;

            const static int NUM_VERT = 1000;
            uniform Vector _MochiBallVertices[NUM_VERT];
            uniform float _Threshold = 0.2;
            uniform float _BaseRadius = 0.7;

            float circle(float2 p, float2 sp, float r)
            {   
                float dist = length(p - sp);

                float alpha = (dist < r)? 0.0 : 1.0;

                fixed4 col = fixed4(0.0, 0.0, 0.0, alpha);
                
                return alpha;
            }

            fixed4 metaBall(float2 p, float2 up, float2 sp)
            {   
                float c = 0;
                float val = 0.008;
                c += val * 0.5 / length(p - up);
                c += val / length(p - sp);
                // c += 0.05 / length(p - float2(1.0, 1.0));

                c = step(0.2, c);

                float  a = (c > 0.0) ? 0.0 : 1.0;
                fixed4 col = fixed4(c, c, c, a);

                return a;
            }

            fixed4 metaBallMulti2(float2 p, float2 up0, float2 sp1, float2 sp2)
            {   
                float c = 0;
                float val = 0.008;
                c += val * 0.5 / length(p - up0);
                c += val / length(p - sp1);
                c += val / length(p - sp2);

                c = step(0.2, c);

                float  a = (c > 0.0) ? 0.0 : 1.0;
                fixed4 col = fixed4(c, c, c, a);

                return a;
            }

            float map(float value, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp)
            {
                float outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);
                
                if( clamp ){
                    if(outputMax < outputMin){
                        if( outVal < outputMax )outVal = outputMax;
                        else if( outVal > outputMin )outVal = outputMin;
                    }else{
                        if( outVal > outputMax )outVal = outputMax;
                        else if( outVal < outputMin )outVal = outputMin;
                    }
                }
                return outVal;
            }

            float2 convertWorldToUV(float x, float z, float planeSize)
            {
                float2 convertedPosition = float2(
                    z * 2.0 / planeSize * -1.0,
                    x * 2.0 / planeSize
                );

                return convertedPosition;
            }

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
                //float2 planeSize = float2(25.35, 21.8);
                float planeSize = 25.35; //色々不具合があるのでPlaneの長辺をとった正方形で整形
                float2 pos = -(i.uv.xy * 2.0 - 1.0);        

                float2 rpos0 = convertWorldToUV(_RobotPos_0.x, _RobotPos_0.z, planeSize); //Unity座標系 単位はｍ
                float2 rpos1 = convertWorldToUV(_RobotPos_1.x, _RobotPos_1.z, planeSize); //Unity座標系 単位はｍ
                //float2 upos0 = convertWorldToUV(_UserPositions[0].x, _UserPositions[0].z, planeSize);
                float2 upos0 = convertWorldToUV(_UserPosition_0.x, _UserPosition_0.z, planeSize);

                float2 uposArray[10];
                for(int i = 0; i < _NUM_USER; i ++)
                {
                    uposArray[i] = convertWorldToUV(_UserPositions[i].x, _UserPositions[i].z, planeSize);
                }
                
                float2 upos1 = convertWorldToUV(_UserPosition_1.x, _UserPosition_1.z, planeSize);

                float2 inputValue = float2(_RobotPos_0.x, _RobotPos_0.z); //Unity座標系 単位はｍ
                
                float inputValueRadius = 0.5000; //Unity座標系 単位はｍ
                float radius = inputValueRadius * 2.0 / planeSize;

                float shaderDistance = _Disntance / planeSize;

                radius = map(shaderDistance, 0.0, 0.30, 0.20, 0.05, true);

                float alpha_1 = circle(pos, rpos0, radius); //要注意、対応づけしていない。
                // float alpha_2 = metaBall(pos, upos0, rpos0);
                // float alpha_3 = metaBallMulti2(pos, uposArray[0], rpos0, rpos1); 
                //float alpha_3 = metaBallMulti2(pos, upos0, rpos0, rpos1); 

                float c = 0;
                float val = 0.008;

                if(0 < _ActiveUserNum && _ActiveUserNum <= _NUM_USER)
                {
                    for(int j = 0; j < _ActiveUserNum; j ++)
                    {
                        c += val * 0.125/ length(pos - uposArray[j]);
                    }
                }
                
                
                c += val / length(pos - rpos0);
                c += val / length(pos - rpos1);

                c = step(0.2, c);

                float  a = (c > 0.0) ? 0.0 : 1.0;

                return fixed4(0.0, 0.0, 0.0, a);

                //複数のロボットについてはAlphaの掛け算で実現なので可能台数分をかけましょう。

            }
            ENDCG
        }
    }
}
