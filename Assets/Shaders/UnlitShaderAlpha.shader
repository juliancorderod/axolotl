Shader "Unlit/Alpha" {
 Properties {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Color (RGB) Alpha (A)", 2D) = "white"
 }
  
 Category {
     Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

     Blend SrcAlpha OneMinusSrcAlpha
     Cull Off Lighting Off ZWrite Off 
    
    
     BindChannels {
         Bind "Color", color
         Bind "Vertex", vertex
         Bind "TexCoord", texcoord
     }
    
     SubShader {
         Pass {
             SetTexture [_MainTex] {
                constantColor [_Color]
                 Combine texture * constant, texture * constant
             }
         }
     }
 }
 }