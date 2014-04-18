﻿using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Jv.Games.DX
{
    public interface IMesh
    {
        SharpDX.Direct3D9.VertexBuffer Vertex { get; }
        SharpDX.Direct3D9.IndexBuffer Index { get; }
        int NumVertices { get; }
        int NumPrimitives { get; }
        SharpDX.Direct3D9.PrimitiveType PrimitiveType { get; }
    }

    public class Mesh<DataType> : IMesh
        where DataType : struct
    {
        public SharpDX.Direct3D9.VertexBuffer Vertex { get; private set; }
        public SharpDX.Direct3D9.IndexBuffer Index { get; private set; }
        public int NumVertices { get; private set; }
        public int NumPrimitives { get; private set; }
        public SharpDX.Direct3D9.PrimitiveType PrimitiveType { get; private set; }

        SharpDX.Direct3D9.Device _device;

        public Mesh(SharpDX.Direct3D9.Device device, SharpDX.Direct3D9.PrimitiveType type)
        {
            PrimitiveType = type;
            _device = device;
        }

        protected DataType[] VertexData
        {
            set
            {
                Vertex = new SharpDX.Direct3D9.VertexBuffer(_device, Marshal.SizeOf(typeof(DataType)) * value.Length, Usage.None, VertexFormat.None, Pool.Managed);
                using (var stream = Vertex.LockData())
                    stream.Data.WriteRange(value);
                NumVertices = value.Length;
            }
        }

        protected int[] IndexData
        {
            set
            {
                Index = new SharpDX.Direct3D9.IndexBuffer(_device, Marshal.SizeOf(typeof(int)) * value.Length, Usage.None, Pool.Managed, false);
                using (var stream = Index.LockData())
                    stream.Data.WriteRange(value);

                switch (PrimitiveType)
                {
                    case SharpDX.Direct3D9.PrimitiveType.TriangleStrip:
                        NumPrimitives = value.Length - 2;
                        break;
                    case SharpDX.Direct3D9.PrimitiveType.TriangleList:
                        NumPrimitives = value.Length / 3;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}