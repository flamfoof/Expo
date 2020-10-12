Shader "Custom/BumpedHighlightSpecular" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _SpecularTextureGloss("Specular(RGB)Gloss(A)", 2D) = "black" {}
        _SpecularMultiple("Specular Multiple", Float) = 1
    }
    SubShader 
    { 
        Tags 
        { 
            "RenderType"="Opaque"  
        }
        LOD 400

        CGPROGRAM
        #pragma surface surf BlinnPhong

        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;
        half _Shininess;
        sampler2D _SpecularTextureGloss;
        half _SpecularMultiple;
        fixed4 _SpecularColor;

        struct Input 
        {
            half2 uv_MainTex;
            half2 uv_BumpMap;
            half2 uv_SpecularTextureGloss;
        };

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 mainTexture = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 specularTexture = tex2D(_SpecularTextureGloss,(IN.uv_SpecularTextureGloss.xyxy).xy);
            fixed4 specular = _Shininess.xxxx + specularTexture.aaaa;
            fixed4 specular2 = specularTexture + _SpecColor;
            fixed4 gloss = specular2 * _SpecularMultiple.xxxx;

            _SpecColor = _SpecColor * specularTexture;

            o.Albedo = mainTexture.rgb * _Color.rgb;
            o.Gloss = gloss;
            o.Alpha = mainTexture.a * _Color.a;
            o.Specular = specular ;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }

        ENDCG
    }

    FallBack "Specular"
}