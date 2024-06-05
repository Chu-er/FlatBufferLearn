// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffers
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct TestUnion : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static TestUnion GetRootAsTestUnion(ByteBuffer _bb) { return GetRootAsTestUnion(_bb, new TestUnion()); }
  public static TestUnion GetRootAsTestUnion(ByteBuffer _bb, TestUnion obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public TestUnion __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public FlatBuffers.num UniType { get { int o = __p.__offset(4); return o != 0 ? (FlatBuffers.num)__p.bb.Get(o + __p.bb_pos) : FlatBuffers.num.NONE; } }
  public TTable? Uni<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(6); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
  public FlatBuffers.num64 UniAsbigNum() { return Uni<FlatBuffers.num64>().Value; }
  public FlatBuffers.num32 UniAssmallNum() { return Uni<FlatBuffers.num32>().Value; }

  public static Offset<FlatBuffers.TestUnion> CreateTestUnion(FlatBufferBuilder builder,
      FlatBuffers.num uni_type = FlatBuffers.num.NONE,
      int uniOffset = 0) {
    builder.StartTable(2);
    TestUnion.AddUni(builder, uniOffset);
    TestUnion.AddUniType(builder, uni_type);
    return TestUnion.EndTestUnion(builder);
  }

  public static void StartTestUnion(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddUniType(FlatBufferBuilder builder, FlatBuffers.num uniType) { builder.AddByte(0, (byte)uniType, 0); }
  public static void AddUni(FlatBufferBuilder builder, int uniOffset) { builder.AddOffset(1, uniOffset, 0); }
  public static Offset<FlatBuffers.TestUnion> EndTestUnion(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffers.TestUnion>(o);
  }
  public TestUnionT UnPack() {
    var _o = new TestUnionT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(TestUnionT _o) {
    _o.Uni = new FlatBuffers.numUnion();
    _o.Uni.Type = this.UniType;
    switch (this.UniType) {
      default: break;
      case FlatBuffers.num.bigNum:
        _o.Uni.Value = this.Uni<FlatBuffers.num64>().HasValue ? this.Uni<FlatBuffers.num64>().Value.UnPack() : null;
        break;
      case FlatBuffers.num.smallNum:
        _o.Uni.Value = this.Uni<FlatBuffers.num32>().HasValue ? this.Uni<FlatBuffers.num32>().Value.UnPack() : null;
        break;
    }
  }
  public static Offset<FlatBuffers.TestUnion> Pack(FlatBufferBuilder builder, TestUnionT _o) {
    if (_o == null) return default(Offset<FlatBuffers.TestUnion>);
    var _uni_type = _o.Uni == null ? FlatBuffers.num.NONE : _o.Uni.Type;
    var _uni = _o.Uni == null ? 0 : FlatBuffers.numUnion.Pack(builder, _o.Uni);
    return CreateTestUnion(
      builder,
      _uni_type,
      _uni);
  }
}

public class TestUnionT
{
  public FlatBuffers.numUnion Uni { get; set; }

  public TestUnionT() {
    this.Uni = null;
  }
}


}
