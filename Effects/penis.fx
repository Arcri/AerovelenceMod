sampler2D input : register(s0);


float SampleInputParam : register(C0);

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	
	float4 color; 
	color= tex2D( input , uv.xy); 

	return color; 
}