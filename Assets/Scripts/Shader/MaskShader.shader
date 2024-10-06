Shader "Unlit/MaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_NDITex("NDI Texture", 2D) = "white" {}
        _MaskChannel("Channel", Int) = 0
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
            //sampler2D _NDITex;
            float4 _MainTex_ST;
            float   _MaskChannel;
            float _MetaValue;
            float _Disntance;

            int _NumRobot;

            Vector _UserPosition;
            Vector _RobotPos_1;
            Vector _RobotPos_2;
            Vector _RobotPos_3;

            const static int NUM_VERT = 1000;
            uniform Vector _MochiBallVertices[NUM_VERT];
            uniform float _Threshold = 0.2;
            uniform float _BaseRadius = 0.7;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 standard(v2f i)
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = 0.0;
                return col;
            }

            fixed4 circle(float2 p, float2 sp, float r)
            {   
                float dist = length(p - sp);

                float alpha = (dist < r)? 0.0 : 1.0;

                fixed4 col = fixed4(0.0, 0.0, 0.0, alpha);
                
                return col;
            }

            float circle_alt(float2 p, float2 sp, float r)
            {
                return length(p - sp) - r;
            }

            
            float rhombus(float2 p, float2 sp, float2 size){
                float pi = 3.14159265359;
                return abs(p.x - sp.x) * (1.0 / (pi / 2.0)) + abs(p.y - sp.y) - size.xy;
            }

            
            float rectangle(float2 p, float2 sp, float2 size){
                return max(abs(p.x - sp.x) - size.x, abs(p.y - sp.y) - size.y);
            }

            fixed4 mochiBall(float2 p, float2 up, float2 sp)
            {
                float pi = 3.14159265359;
                float two_pi = 6.28318530718;
                
                float thetaSumUp = 0.0;
                float alpha = 0.0;
                float radius = 0.25;
                float interpolation = 0.0;
                float step = two_pi / NUM_VERT;

                for(int i = 0; i < NUM_VERT; i ++)
                {
                    int _i = (i + 1) % NUM_VERT;
                    float2 v = float2(_MochiBallVertices[i].x, _MochiBallVertices[i].y);
                    float2 _v = float2(_MochiBallVertices[_i].x, _MochiBallVertices[_i].y);
                    float2 transCurrentVert = v - p;
                    float2 transNextVert = _v - p;

                    interpolation = dot(normalize(transCurrentVert), normalize(transNextVert));
                    thetaSumUp += acos(interpolation);

                }
                
                alpha = (thetaSumUp > two_pi * 0.9990) ? 0.0 : 1.0;
                
                fixed4 col = fixed4(0.0, 0.0, 0.0, alpha);
                return col;
            }

            fixed4 metaBall(float2 p, float2 up, float2 sp)
            {   
                float c = 0;
                float val = 0.022;
                c += val * 0.5 / length(p - up);
                c += val / length(p - sp);
                // c += 0.05 / length(p - float2(1.0, 1.0));

                c = step(0.2, c);

                float  a = (c > 0.0) ? 0.0 : 1.0;
                fixed4 col = fixed4(c, c, c, a);

                return col;
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

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = -(i.uv.xy * 2.0 - 1.0) * 2.0;
                float2 upos = float2(_UserPosition.x, _UserPosition.z) * 0.313;
                float2 r1pos = float2(_RobotPos_1.x, _RobotPos_1.z) * 0.313;
                float shaderDistance = _Disntance * 0.313;
                float shaderThreshold = _Threshold * 0.313;
                float shaderChangeScale = _Threshold * 1.0  * 0.313;
                float baseSize = _BaseRadius * 0.313;
                float scaleChangeRatio = 0.6;
                float radius = baseSize;
                float thresholdAdjust = 1.0;

                switch(_MaskChannel){
                    //1.静止条件
                    case 1:
                        return circle(pos, r1pos, radius);
                    break;

                    //2.離散拡大
                    case 2:
                        radius = (shaderDistance < shaderThreshold)? baseSize * scaleChangeRatio : baseSize;
                        return circle(pos, r1pos, radius);
                    break;

                    //3.離散縮小
                    case 3:
                        radius = (shaderDistance > shaderThreshold)? baseSize * scaleChangeRatio : baseSize;
                        return circle(pos, r1pos, radius);
                    break;

                    //4.連続拡大
                    case 4:
                        radius = map(shaderDistance, (shaderThreshold - shaderChangeScale) * thresholdAdjust, shaderThreshold * thresholdAdjust, baseSize * scaleChangeRatio, baseSize, true);
                        return circle(pos, r1pos, radius);
                    break;

                    //5.連続縮小
                    case 5:
                        radius = map(shaderDistance, (shaderThreshold - shaderChangeScale) * thresholdAdjust, shaderThreshold * thresholdAdjust, baseSize, baseSize * scaleChangeRatio, true);
                        return circle(pos, r1pos, radius);
                    break;
                    
                    //6.モチ
                    // case 6:
                    //     return mochiBall(pos, upos, r1pos);
                    // break;

                    //7.メタ
                    case 6:
                        return metaBall(pos, upos, r1pos);
                    break;

                    //8.モーフィング-1 変形-移動アリ
                    case 7:
                        float fixedDistance = map(shaderDistance, (shaderThreshold - shaderChangeScale) * thresholdAdjust, shaderThreshold * thresholdAdjust, 1.0, 0.0, true);
                        float cir = circle_alt(pos, r1pos, -radius);
                        float cir_u = circle_alt(pos, upos, -radius);
                       // float rec = rectangle(r1pos, pos, float2(-radius, -radius));
                        fixedDistance = smoothstep(0.1, 0.9, fixedDistance);
                        float morph = lerp(cir, cir_u, fixedDistance);
                        fixed4 morphCol = fixed4(0.0, 0.0, 0.0, smoothstep(radius * 2 - 0.001, radius * 2 ,morph));
                        return morphCol;
                    break;
                    
                    //9.モーフィング-2 変形-移動ナシ
                    case 8:
                        float fixedDistance_ = map(shaderDistance, (shaderThreshold - shaderChangeScale) * thresholdAdjust, shaderThreshold * thresholdAdjust, 1.0, 0.0, true);
                        float cir_ = circle_alt(pos, r1pos, -radius);
                       // float cir_u = circle_alt(pos, upos, -radius);
                        //float rec = rhombus(r1pos, pos, float2(-radius * 1.3, -radius * 1.3));
                        float rec = rhombus(r1pos, pos, float2(-radius * 1.0, -radius * 1.0));
                        float morph_ = lerp(cir_, rec, fixedDistance_);
                        fixed4 morphCol_ = fixed4(0.0, 0.0, 0.0, smoothstep(radius * 2, radius * 2 + 0.001, morph_));
                        return morphCol_;

                    break;

                    default:                   
                        return standard(i);
                    break;
                }
                
            }
            ENDCG
        }
    }
}
