Shader "Psychonauts/Psychonauts-Additive"
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
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" }
        LOD 100
        Lighting Off
        ZWrite Off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "PsychonautsShared.cginc"

            v2f vert(appdata v) {
                return process_vert(v, 0.0);
            }

            fixed4 frag(v2f i) : SV_Target{
                return process_frag(i, 0.0, 0.0, 0.001);
            }
            ENDCG
        }
    }
}
