﻿Shader "Psychonauts/Psychonauts-AlphaTest"
{
    Properties
    {
        _Tex0("Texture", 2D) = "white" {}
        _Tex1("Texture", 2D) = "white" {}
        _Tex2("Texture", 2D) = "white" {}
        _TexLightMap("Texture", 2D) = "white" {}

        _TexGlossMap("Texture", 2D) = "white" {}
        _TexReflectionMap("Texture", 2D) = "white" {}

        _TexturesInUse("Textures In Use", Vector) = (0,0,0,0) // 0, 1, 2, LightMap
        _TexturesInUse2("Textures In Use", Vector) = (0,0,0,0) // GlossMap, ReflectionMap
        [MaterialToggle] _IsSelfIllumination("Is Self-Illuminated", Float) = 0
        _MaterialColor("Material Color", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        Lighting Off
		Pass{
			ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma target 3.0
            #pragma vertex vert  
            #pragma fragment frag 
            #pragma multi_compile_fog

            #include "PsychonautsShared.cginc"

            v2f vert(appdata v) {
                return process_vert(v, 0.0);
            }

            fixed4 frag(v2f i) : SV_Target{
                return process_frag(i, 1.0, 0.0, 0.6);
            }
            ENDCG
        }
        Pass{
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma target 3.0
            #pragma vertex vert  
            #pragma fragment frag 

            #include "PsychonautsShared.cginc"

            v2f vert(appdata v) {
                return process_vert(v, 0.0);
            }

            fixed4 frag(v2f i) : SV_Target{
                return process_frag(i, -1.0, 0.0, 0.6);
            }
            ENDCG
        }
    }
}
