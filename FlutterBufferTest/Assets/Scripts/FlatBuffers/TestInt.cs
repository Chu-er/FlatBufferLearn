// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffers
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct TestInt : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static TestInt GetRootAsTestInt(ByteBuffer _bb) { return GetRootAsTestInt(_bb, new TestInt()); }
  public static TestInt GetRootAsTestInt(ByteBuffer _bb, TestInt obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public TestInt __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Scalar { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateScalar(int scalar) { int o = __p.__offset(4); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, scalar); return true; } else { return false; } }

  public static Offset<FlatBuffers.TestInt> CreateTestInt(FlatBufferBuilder builder,
      int scalar = 0) {
    builder.StartTable(1);
    TestInt.AddScalar(builder, scalar);
    return TestInt.EndTestInt(builder);
  }

  public static void StartTestInt(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddScalar(FlatBufferBuilder builder, int scalar) { builder.AddInt(0, scalar, 0); }
  public static Offset<FlatBuffers.TestInt> EndTestInt(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffers.TestInt>(o);
  }
  public TestIntT UnPack() {
    var _o = new TestIntT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(TestIntT _o) {
    _o.Scalar = this.Scalar;
  }
  public static Offset<FlatBuffers.TestInt> Pack(FlatBufferBuilder builder, TestIntT _o) {
    if (_o == null) return default(Offset<FlatBuffers.TestInt>);
    return CreateTestInt(
      builder,
      _o.Scalar);
  }
}

public class TestIntT
{
  public int Scalar { get; set; }

  public TestIntT() {
    this.Scalar = 0;
  }
}


}
