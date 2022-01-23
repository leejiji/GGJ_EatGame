Shader "Custom/Outline"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Scale("Scale", Float) = 0.01

	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }



			cull front

			CGPROGRAM

#pragma surface surf Nolight vertex:vert noshadow noambient

		float _Scale;


		void vert(inout appdata_full v) {
		v.vertex.xyz = v.vertex.xyz + v.normal.xyz * (_Scale / 1000);
}

	struct Input {
		float4 color:COLOR;
};
	fixed4 _Color;
	void surf(Input IN, inout SurfaceOutput o)
	{

	}


	float4 LightingNolight(SurfaceOutput s,float3 lightDir,float atten)
	{
		return float4(1, 1, 1, 1) * _Color;
	}
		ENDCG
	}
		FallBack "Diffuse"
}
