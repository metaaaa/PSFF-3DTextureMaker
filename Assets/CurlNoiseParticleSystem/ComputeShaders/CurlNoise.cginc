#include "SimplexNoise3D.cginc"

float3 curlNoise(float3 p, float e = 0.009765625)
{
    float3 dx = float3( e   , 0.0 , 0.0 );
    float3 dy = float3( 0.0 , e   , 0.0 );
    float3 dz = float3( 0.0 , 0.0 , e   );

    float3 p_x0 = snoise3d( p - dx );
    float3 p_x1 = snoise3d( p + dx );
    float3 p_y0 = snoise3d( p - dy );
    float3 p_y1 = snoise3d( p + dy );
    float3 p_z0 = snoise3d( p - dz );
    float3 p_z1 = snoise3d( p + dz );

    float x = p_y1.z - p_y0.z - p_z1.y + p_z0.y;
    float y = p_z1.x - p_z0.x - p_x1.z + p_x0.z;
    float z = p_x1.y - p_x0.y - p_y1.x + p_y0.x;

    return normalize( float3( x , y , z ) );
}