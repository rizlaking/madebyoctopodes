﻿Shader "Custom/TestAlphaShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="AlphaTest"}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			// o.Alpha = c.a;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
