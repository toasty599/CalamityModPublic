sampler mainImage : register(s0);
sampler soulsImage : register(s1);
sampler eyesImage : register(s2);
sampler noiseImage : register(s3);
sampler colorImage : register(s4);

float time;
float timeOffset;
float opacity;

float2 screenSize;

float3 eyeNoiseColor;
float3 edgeNoiseColor1;
float3 edgeNoiseColor2;

float InverseLerp(float from, float to, float x)
{
	return saturate((x - from) / (to - from));
}

float sine01to101(float value)
{
	return sin(3.1415 * saturate(value));
}

float3 fbm(float2 uv, sampler2D samplerToUse)
{
	float frequency = 0.5;
	float amplitude = 0.7;
	float amplitudeScale = 0;
	float3 result = 0;
	for (int i = 0; i < 4; i++)
	{
		result += tex2D(samplerToUse, uv * frequency) * amplitude;
		uv += float2(300, 300);
		amplitudeScale += amplitude;
		frequency *= 1.18;
		amplitude *= 0.8;
	}
	return result / amplitudeScale;
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
	// Center the coords and get the distance to the middle of the texture.
	float2 adjustedUV = uv * screenSize;
	adjustedUV = (adjustedUV - 0.5 * screenSize.xy) / screenSize.y;    
    float distanceToCenter = distance(uv, float2(0.5, 0.5)) * lerp(10, 1, opacity);//* (2 - opacity);
    
    // Adjust the time.
    float adjustedTime = time + timeOffset;
    
	// Get the mask thresholds.
    float maskLength = length(uv - 0.5) * lerp(10, 1, opacity);
	float maskOuter = smoothstep(0.4, 0.33, maskLength);
	float maskInner = smoothstep(0.05, 0.33, maskLength);
	
	// Convert to polar coordinates, and make them swirl around the center.
	float2 polar = float2(length(adjustedUV), atan2(adjustedUV.y, adjustedUV.x));
	float2 polarInner = polar;
    polar.y += polar.x * 2 - adjustedTime;
    polar.x = sin(polar.x * 6.283 + adjustedTime * .1) * 0.5 + 0.5;
    polarInner.y += polarInner.x * 10 - adjustedTime;
    polarInner.x = sin(polarInner.x * 6.283 + adjustedTime * 0.8) * 0.5 + 0.5;

	// Convert back to texture coordinates (Cartesian) for texture sampling.
	float2 alteredUV = float2(cos(polar.y), sin(polar.y)) * polar.x;
	float2 alteredUV2 = float2(cos(polarInner.y), sin(polarInner.y)) * polarInner.x;
	
	// Create the main soul vortex.
	float3 soulNoise = tex2D(soulsImage, alteredUV * 0.4).rgb; 
	float4 finalSoul = float4(soulNoise * maskInner * 1.5, 1) * maskOuter;
	
	// Create the highlight streaks of fast moving souls falling into the vortex.
	float eyeNoise = tex2D(eyesImage, alteredUV2 * 0.35).r * 0.7; 
	float edgeNoise = fbm(alteredUV * 0.3, noiseImage);
    eyeNoise *= sine01to101(1 - InverseLerp(0.0, 0.25, maskLength)) * lerp(0.5, 4, saturate((1. + sin(adjustedTime * 3.1415) * 0.5 + edgeNoise * 0.1)));
	float4 finalEyes = float4(lerp(eyeNoiseColor, float3(1., 0.1, 0.1), edgeNoise) * eyeNoise, 1.) * smoothstep(0.4, 0.38, maskLength);
	
	// Create the wispy border of the vortex.
	float distanceNoise = fbm(alteredUV * 1.6, colorImage);
	float3 edgeColor = lerp(lerp(edgeNoiseColor1, edgeNoiseColor2, lerp(0.4, 0.7, InverseLerp(0.55, 0.65, edgeNoise))), edgeNoiseColor1, 0.3);
	float4 finalEdge = float4(edgeColor, 1.) * sine01to101(InverseLerp(0.385, 0.52, distanceToCenter + distanceNoise * 0.15 * opacity)) * 2;
	
    return (finalSoul + finalEyes + finalEdge) * opacity;
}

technique Technique1
{
	pass AutoloadPass
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}

}