// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffers
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct num32 : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static num32 GetRootAsnum32(ByteBuffer _bb) { return GetRootAsnum32(_bb, new num32()); }
  public static num32 GetRootAsnum32(ByteBuffer _bb, num32 obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public num32 __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int TestInt { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateTestInt(int testInt) { int o = __p.__offset(4); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, testInt); return true; } else { return false; } }

  public static Offset<FlatBuffers.num32> Createnum32(FlatBufferBuilder builder,
      int testInt = 0) {
    builder.StartTable(1);
    num32.AddTestInt(builder, testInt);
    return num32.Endnum32(builder);
  }

  public static void Startnum32(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddTestInt(FlatBufferBuilder builder, int testInt) { builder.AddInt(0, testInt, 0); }
  public static Offset<FlatBuffers.num32> Endnum32(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffers.num32>(o);
  }
  public num32T UnPack() {
    var _o = new num32T();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(num32T _o) {
    _o.TestInt = this.TestInt;
  }
  public static Offset<FlatBuffers.num32> Pack(FlatBufferBuilder builder, num32T _o) {
    if (_o == null) return default(Offset<FlatBuffers.num32>);
    return Createnum32(
      builder,
      _o.TestInt);
  }
}

public class num32T
{
  public int TestInt { get; set; }

  public num32T() {
    this.TestInt = 0;
  }
}


}