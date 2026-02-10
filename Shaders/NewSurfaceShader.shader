Shader "Custom/LadrillosMatematicos"
{
    Properties
    {
        _BrickColor ("Color Ladrillo", Color) = (0.7, 0.3, 0.1, 1)
        _MortarColor ("Color Cemento", Color) = (0.6, 0.6, 0.6, 1)
        _BrickSize ("Escala General", Float) = 5.0
        _BrickRatio ("Relacion de Aspecto (2=Normal)", Float) = 2.0
        _MortarSize ("Grosor Cemento", Range(0.01, 0.2)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Añadimos 'addshadow' para que el relieve proyecte sombra propia si es posible
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _BrickColor;
        fixed4 _MortarColor;
        float _BrickSize;
        float _BrickRatio; // Nueva variable para controlar cuan "ancho" es el ladrillo
        float _MortarSize;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // --- CORRECCIÓN DE LA FORMA ---
            // Multiplicamos Y por el Ratio. Si Ratio es 2, habrá el doble de filas,
            // haciendo que cada ladrillo se vea la mitad de alto (más rectangular).
            float2 uv = IN.uv_MainTex * _BrickSize;
            uv.y *= _BrickRatio; 

            // --- CÁLCULO DEL PATRÓN (Igual que antes) ---
            float rowNumber = floor(uv.y);
            // Usamos una lógica un poco más segura para el impar/par
            float isOddRow = step(1.0, fmod(rowNumber, 2.0));
            uv.x += isOddRow * 0.5;

            float2 brickUV = frac(uv);

            // --- BORDES Y COLOR ---
            float2 border = step(_MortarSize, brickUV) - step(1.0 - _MortarSize, brickUV);
            float isBrick = border.x * border.y;

            o.Albedo = lerp(_MortarColor.rgb, _BrickColor.rgb, isBrick);
            
            // --- MEJORA VISUAL: RELIEVE FALSO ---
            // Esto hace que el shader calcule como rebota la luz en los bordes
            // Si es ladrillo es plano (0,0,1), si es borde simulamos una curva
            if (isBrick < 0.5) {
                // Estamos en el cemento, hundimos un poco la normal
                o.Normal = UnpackNormal(float4(0, 0.5, 0, 1)); // Truco visual simple
                o.Smoothness = 0.1;
            } else {
                o.Normal = float3(0,0,1); // Ladrillo plano
                o.Smoothness = 0.6;
            }

            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}