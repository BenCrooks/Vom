// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PixelColors" {
    Properties
    {
        // [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorTint ("Tint", Color) = (1,1,1,1)
        _Color1in ("Color In 1", Color) = (1,1,1,1)
        _Color1out ("Color Out 1", Color) = (1,1,1,1)
        _Color2in ("Color In 2", Color) = (1,1,1,1)
        _Color2out ("Color Out 2", Color) = (1,1,1,1)
        _Color3in ("Color In 3", Color) = (1,1,1,1)
        _Color3out ("Color Out 3", Color) = (1,1,1,1)
        _Color4in ("Color In 4", Color) = (1,1,1,1)
        _Color4out ("Color Out 4", Color) = (1,1,1,1)
        _Color5in ("Color In 5", Color) = (1,1,1,1)
        _Color5out ("Color Out 5", Color) = (1,1,1,1)
        _ReplaceColorCutoff("Color Replace Threshold", float) = 0
        [MaterialToggle] _Flashing ("Flashing", Float) = 0
        _FlashBrightness ("Flash Brightness", Float) = 8
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag        
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"
 
         sampler2D _MainTex;         
         
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
         
            fixed4 _ColorTint;
            fixed4 _Color1in;
            fixed4 _Color1out;
            fixed4 _Color2in;
            fixed4 _Color2out;
            fixed4 _Color3in;
            fixed4 _Color3out;
            fixed4 _Color4in;
            fixed4 _Color4out;
            fixed4 _Color5in;
            fixed4 _Color5out;
            float _ReplaceColorCutoff;
            float _Flashing;
            float _FlashBrightness;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;            
                OUT.color = IN.color * _ColorTint;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
 
                return OUT;
            }
       
         
            fixed4 frag(v2f IN) : COLOR
            {
                float4 texColor = tex2D( _MainTex, IN.texcoord );
                texColor = all(texColor + _ReplaceColorCutoff >= _Color1in && texColor - _ReplaceColorCutoff <= _Color1in) ? _Color1out : texColor;
                texColor = all(texColor + _ReplaceColorCutoff >= _Color2in && texColor - _ReplaceColorCutoff <= _Color2in) ? _Color2out : texColor;
                texColor = all(texColor + _ReplaceColorCutoff >= _Color3in && texColor - _ReplaceColorCutoff <= _Color3in) ? _Color3out : texColor;
                texColor = all(texColor + _ReplaceColorCutoff >= _Color4in && texColor - _ReplaceColorCutoff <= _Color4in) ? _Color4out : texColor;
                texColor = all(texColor + _ReplaceColorCutoff >= _Color5in && texColor - _ReplaceColorCutoff <= _Color5in) ? _Color5out : texColor;

                if (_Flashing == 1)
                    texColor = texColor * float4(_FlashBrightness, _FlashBrightness, _FlashBrightness, _FlashBrightness);

                return texColor * _ColorTint; //apply the tint
            }
        ENDCG
        }
    }
}