Shader "Custom/Toon Enviroment"
{
    Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump"{}
		_ShadowSharp("Shadow Sharpness",Range(0, 1)) = 0.3
		[HDR]
		_AmbientColor("Ambient Color",Color) = (0.4,0.4,0.4,1)
		_AmbientAmount("Ambient Amount",float) = 0.1
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			
			float4 _Color;
			float4 _AmbientColor;
			float _AmbientAmount;
			float _ShadowSharp;

			struct appdata
			{
				float3 normal : NORMAL;
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float4 bumpMap : TEXCOORD6;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				SHADOW_COORDS(2)
				half3 xTangent : TEXCOORD3;
				half3 yTangent : TEXCOORD4;
				half3 zTangent : TEXCOORD5;
				float2 bumpMap : TEXCOORD6;
				float3 worldNormal : NORMAL;
				float4 pos : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.bumpMap = TRANSFORM_TEX(v.bumpMap, _BumpMap);

				half3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBitangent = cross(o.worldNormal, worldTangent) * tangentSign;
				o.xTangent = half3(worldTangent.x,worldBitangent.x, o.worldNormal.x);
				o.yTangent = half3(worldTangent.y,worldBitangent.y, o.worldNormal.y);
				o.zTangent = half3(worldTangent.z,worldBitangent.z, o.worldNormal.z);

				TRANSFER_SHADOW(o)
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.bumpMap));
				float3 worldNormal;
				worldNormal.x = dot(i.xTangent, tnormal);
				worldNormal.y = dot(i.yTangent, tnormal);
				worldNormal.z = dot(i.zTangent, tnormal);

				float3 normal = normalize(worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float shadow = SHADOW_ATTENUATION(i);
				shadow = step(_ShadowSharp, shadow);
				shadow += (1-shadow)* _AmbientAmount;

				float lightIntensity = max(NdotL, _AmbientAmount);
				float4 light = lightIntensity * _LightColor0;

				float3 viewDir = normalize(i.viewDir);
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = max(dot(normal, halfVector),0);

				float4 sample = tex2D(_MainTex, i.uv);

				//return _Color * sample * light;
				float4 lightData = _Color * sample * light;
				lightData *= shadow;

				return lightData;
				//return float4(worldNormal.x,worldNormal.y,worldNormal.z,1);
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		//FallBack "Diffuse"
	}
}
