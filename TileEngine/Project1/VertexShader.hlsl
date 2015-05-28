
cbuffer Args0 : register(b0)
{
    float4x4 p;


};

cbuffer Args1 : register(b1)
{
	float4x4 v;
};

cbuffer Args2 : register(b2)
{
	float4x4 w;
	float3 glbTrans; //a Global translation
	float cs;// cellsize
	float2 txcrdbase;
	

};



struct surface
{
	float3 trans;
	float3 color;
	uint index;
};


StructuredBuffer<surface> surfaces : register(t0);
StructuredBuffer<float2> texcoords : register(t1);



struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;
};



VS_OUT main( uint v_id : SV_VertexID ) 
{

	/*
			0,0--1,0	
             |    |
			0,2--1,2	
	*/
	//float4 postex = (v_id & 2)?/*<2>*/((v_id % 2 )? /*1,2*/float4(1,-1,  1,1):/*0,2*/ float4(-1,-1,  0,1) ):/*<0>*/((v_id % 2)?/*1,0*/float4(1,1,  1,0) :/*0,0*/float4(-1,1,  0,0));
	
	
	//I don't need this for the positions!
	//dynamic quad                                                //pos,  tex                   pos,  tex                                     pos,  tex                  pos,  tex
	float4 postex = (v_id & 2)?
	((v_id % 2 )? /*<2>*/
		float4(cs,cs,  1,1): /*1,2*/
	    float4(-cs,cs,  0,1) ):/*0,2*/
	((v_id % 2)?/*<0>*/
		float4(cs,-cs,  1,0) :/*1,0*/
	    float4(-cs,-cs,  0,0));/*0,0*/


	float3 pos = float3(postex.x,postex.y,0);
		



    uint offsetID = v_id / 4;
	float3 w_pos = mul( glbTrans + surfaces[offsetID].trans,w);



	float3 wv_pos = mul( pos + w_pos,v);
	
	VS_OUT output;
	
	output.pos = float4(mul(wv_pos,p));
	output.pos.w = 1;
	
	uint id = surfaces[offsetID].index;
	

	output.col = float4(surfaces[offsetID].color,1);

	output.tex = float2(postex.z == 0 ?texcoords[surfaces[offsetID].index].x :texcoords[surfaces[offsetID].index].x + txcrdbase.x, postex.w == 0?texcoords[surfaces[offsetID].index].y:texcoords[surfaces[offsetID].index].y + txcrdbase.y);


	return output;
}