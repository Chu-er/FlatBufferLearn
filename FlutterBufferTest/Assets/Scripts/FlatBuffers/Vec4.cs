// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffers
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Vec4 : IFlatbufferObject
{
  private Struct __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public void __init(int _i, ByteBuffer _bb) { __p = new Struct(_i, _bb); }
  public Vec4 __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public FlatBuffers.Vec3 Xyz { get { return (new FlatBuffers.Vec3()).__assign(__p.bb_pos + 0, __p.bb); } }
  public float W { get { return __p.bb.GetFloat(__p.bb_pos + 12); } }
  public void MutateW(float w) { __p.bb.PutFloat(__p.bb_pos + 12, w); }

  public static Offset<FlatBuffers.Vec4> CreateVec4(FlatBufferBuilder builder, float xyz_X, float xyz_Y, float xyz_Z, float W) {
    builder.Prep(4, 16);
    builder.PutFloat(W);
    builder.Prep(4, 12);
    builder.PutFloat(xyz_Z);
    builder.PutFloat(xyz_Y);
    builder.PutFloat(xyz_X);
    return new Offset<FlatBuffers.Vec4>(builder.Offset);
  }
  public Vec4T UnPack() {
    var _o = new Vec4T();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(Vec4T _o) {
    _o.Xyz = this.Xyz.UnPack();
    _o.W = this.W;
  }
  public static Offset<FlatBuffers.Vec4> Pack(FlatBufferBuilder builder, Vec4T _o) {
    if (_o == null) return default(Offset<FlatBuffers.Vec4>);
    var _xyz_x = _o.Xyz.X;
    var _xyz_y = _o.Xyz.Y;
    var _xyz_z = _o.Xyz.Z;
    return CreateVec4(
      builder,
      _xyz_x,
      _xyz_y,
      _xyz_z,
      _o.W);
  }
}

public class Vec4T
{
  public FlatBuffers.Vec3T Xyz { get; set; }
  public float W { get; set; }

  public Vec4T() {
    this.Xyz = new FlatBuffers.Vec3T();
    this.W = 0.0f;
  }
}


}
