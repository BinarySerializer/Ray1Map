Shader "Psychonauts/Psychonauts-Decal"
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
    }
   
    SubShader
    {
        Lighting Off
        ZTest LEqual
        ZWrite Off
        Tags {"Queue" = "Transparent"}
        Pass
        {
            Alphatest Greater 0
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

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
        }
    }
}