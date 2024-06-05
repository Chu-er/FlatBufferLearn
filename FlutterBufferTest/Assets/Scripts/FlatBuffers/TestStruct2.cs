// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffers
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct TestStruct2 : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static TestStruct2 GetRootAsTestStruct2(ByteBuffer _bb) { return GetRootAsTestStruct2(_bb, new TestStruct2()); }
  public static TestStruct2 GetRootAsTestStruct2(ByteBuffer _bb, TestStruct2 obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public TestStruct2 __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public FlatBuffers.Vec4? Quaternion { get { int o = __p.__offset(4); return o != 0 ? (FlatBuffers.Vec4?)(new FlatBuffers.Vec4()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartTestStruct2(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddQuaternion(FlatBufferBuilder builder, Offset<FlatBuffers.Vec4> quaternionOffset) { builder.AddStruct(0, quaternionOffset.Value, 0); }
  public static Offset<FlatBuffers.TestStruct2> EndTestStruct2(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffers.TestStruct2>(o);
  }
  public TestStruct2T UnPack() {
    var _o = new TestStruct2T();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(TestStruct2T _o) {
    _o.Quaternion = this.Quaternion.HasValue ? this.Quaternion.Value.UnPack() : null;
  }
  public static Offset<FlatBuffers.TestStruct2> Pack(FlatBufferBuilder builder, TestStruct2T _o) {
    if (_o == null) return default(Offset<FlatBuffers.TestStruct2>);
    StartTestStruct2(builder);
    AddQuaternion(builder, FlatBuffers.Vec4.Pack(builder, _o.Quaternion));
    return EndTestStruct2(builder);
  }
}

public class TestStruct2T
{
  public FlatBuffers.Vec4T Quaternion { get; set; }

  public TestStruct2T() {
    this.Quaternion = new FlatBuffers.Vec4T();
  }
}


}
