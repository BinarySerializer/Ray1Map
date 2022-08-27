Shader "Psychonauts/Psychonauts-Cutout"
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

        _Cutoff("Alpha cutoff", Range(0,1)) = 0.1
    }
    SubShader
    {
        /*Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "PsychonautsShared.cginc"
            float _Cutoff;

            v2f vert(appdata v) {
                return process_vert(v, 0.0);
            }

            fixed4 frag(v2f i) : SV_Target{
                return process_frag(i, _Cutoff, 0.0);
            }
            ENDCG
        }*/

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Lighting Off
		Pass{
			ZWrite Off

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
                return process_frag(i, -1.0, 0.0);
            }
			ENDCG
		}

		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		Pass{
			ZWrite On
			Blend One Zero

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert  
			#pragma fragment frag 

            #include "PsychonautsShared.cginc"

            v2f vert(appdata v) {
                return process_vert(v, 0.0);
            }

            fixed4 frag(v2f i) : SV_Target{
                return process_frag(i, 1.0, 0.0);
            }
			ENDCG
		}
    }
}
