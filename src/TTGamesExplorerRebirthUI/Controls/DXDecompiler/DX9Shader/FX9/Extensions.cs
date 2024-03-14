using DXDecompiler.DX9Shader.Bytecode.Ctab;
using DXDecompiler.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace DXDecompiler.DX9Shader.FX9
{
    public static class Extensions
    {
        public static bool IsSampler(this ParameterType type)
        {
            return type switch
            {
                ParameterType.Sampler or ParameterType.Sampler1D or ParameterType.Sampler2D or ParameterType.Sampler3D or ParameterType.SamplerCube => true,
                _ => false,
            };
        }
        public static bool IsObjectType(this ParameterType type)
        {
            return type switch
            {
                ParameterType.Texture or ParameterType.PixelShader or ParameterType.VertexShader => true,
                _ => false,
            };
        }
        public static bool HasVariableBlob(this ParameterType type)
        {
            return type switch
            {
                ParameterType.Texture or ParameterType.Texture1D or ParameterType.Texture2D or ParameterType.Texture3D or ParameterType.TextureCube or 
                ParameterType.PixelShader or ParameterType.VertexShader or ParameterType.String => true,
                _ => false,
            };
        }
        public static bool HasStateBlob(this StateType type)
        {
            return type switch
            {
                StateType.Texture or StateType.Sampler or StateType.VertexShader or StateType.PixelShader => true,
                _ => false,
            };
        }
        public static bool RequiresIndex(this StateType type)
        {
            return type switch
            {
                StateType.ColorOp or StateType.ColorArg0 or StateType.ColorArg1 or StateType.ColorArg2 or StateType.AlphaOp or StateType.AlphaArg0 or 
                StateType.AlphaArg1 or StateType.AlphaArg2 or StateType.ResultArg or StateType.BumpEnvMat00 or StateType.BumpEnvMat01 or StateType.BumpEnvMat10 or 
                StateType.BumpEnvMat11 or StateType.TexCoordIndex or StateType.BumpEnvLScale or StateType.BumpEnvLOffset or StateType.TextureTransformFlags or 
                StateType.Constant or StateType.TextureTransform or StateType.LightType or StateType.LightDiffuse or StateType.LightSpecular or StateType.LightAmbient or 
                StateType.LightPosition or StateType.LightDirection or StateType.LightRange or StateType.LightFalloff or StateType.LightAttenuation0 or 
                StateType.LightAttenuation1 or StateType.LightAttenuation2 or StateType.LightTheta or StateType.LightPhi or StateType.LightEnable or 
                StateType.Texture or StateType.AddressU or StateType.AddressV or StateType.AddressW or StateType.BorderColor or StateType.MagFilter or 
                StateType.MinFilter or StateType.MipFilter or StateType.MipMapLodBias or StateType.MaxMipLevel or StateType.MaxAnisotropy or StateType.Sampler => true,
                _ => false,
            };
        }
        public static string TryReadString(this BytecodeReader reader)
        {
            try
            {
                var length = reader.ReadUInt32();
                if (length == 0)
                {
                    return "";
                }
                var bytes = reader.ReadBytes((int)length);
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length - 1);
            }
            catch (Exception)
            {
                return "Error reading string";
            }
        }
        public static List<Number> ReadParameterValue(this Parameter parameter, BytecodeReader valueReader)
        {
            var result = new List<Number>();
            if (parameter.ParameterClass == ParameterClass.Object)
            {
                var elementCount = parameter.ElementCount == 0 ? 1 : parameter.ElementCount;
                for (int i = 0; i < elementCount; i++)
                {
                    result.Add(Number.Parse(valueReader));
                }
            }
            else
            {
                var defaultValueCount = parameter.GetSize() / 4;
                for (int i = 0; i < defaultValueCount; i++)
                {
                    result.Add(Number.Parse(valueReader));
                }
            }
            return result;
        }
    }
}