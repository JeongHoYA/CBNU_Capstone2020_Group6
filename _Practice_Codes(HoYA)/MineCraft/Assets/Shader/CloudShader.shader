﻿Shader "Minecraft/CloudShader"
{
	properties
	{
		_Color("Colour", Color) = (1, 1, 1, 1)
	}
	
	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"= "Transparent"}

		ZWrite Off
		Lighting Off
		Fog { Mode Off }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Greater
				Pass Incrsat
			}

			Color[_Color]
		}
	}
}
