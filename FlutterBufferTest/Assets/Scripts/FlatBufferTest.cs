using System.Collections.Generic;
using System.IO;
using LIBII;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace FlatBuffers
{
    public class FlatBufferTest : MonoBehaviour
    {
        /*#region Byte

        [Title("Byte")] public sbyte testByte;

        private TestByteT _testByte;

        [Button(), Horizontal("TestByte")]
        public void ReadByte()
        {
            var path = "Assets/TestByte.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testByte = TestByteT.DeserializeFromBinary(buffer);
            }
            else
            {
                _testByte = new TestByteT();
            }
            testByte = _testByte.Scalar;
        }

        [Button(), Horizontal("TestByte")]
        public void WriteByte()
        {
            var path = "Assets/TestByte.bytes";
            _testByte ??= new TestByteT();
            _testByte.Scalar = testByte;
            File.WriteAllBytes(path, _testByte.SerializeToBinary());
        }

        #endregion*/

        /*#region Int

        [Title("Int")] public int testInt;

        private TestIntT _testInt;

        [Button(), Horizontal("TestInt")]
        public void ReadInt()
        {
            var path = "Assets/TestInt.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testInt = TestIntT.DeserializeFromBinary(buffer);
            }
            else
            {
                _testInt = new TestIntT();
            }
            testInt = _testInt.Scalar;
        }

        [Button(), Horizontal("TestInt")]
        public void WriteInt()
        {
            var path = "Assets/TestInt.bytes";
            _testInt ??= new TestIntT();
            _testInt.Scalar = testInt;
            File.WriteAllBytes(path, _testInt.SerializeToBinary());
        }

        #endregion*/

        /*#region Bool

        [Title("Bool")] public bool testBool;

        private TestBoolT _testBool;

        [Button(), Horizontal("TestBool")]
        public void ReadBool()
        {
            var path = "Assets/TestBool.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testBool = TestBoolT.DeserializeFromBinary(buffer);
            }
            else
            {
                _testBool = new TestBoolT();
            }
            testBool = _testBool.Scalar;
        }

        [Button(), Horizontal("TestBool")]
        public void WriteBool()
        {
            var path = "Assets/TestBool.bytes";
            _testBool ??= new TestBoolT();
            _testBool.Scalar = testBool;
            File.WriteAllBytes(path, _testBool.SerializeToBinary());
        }

        #endregion*/

        /*#region Array

        [Title("Array")] public List<int> testArray;

        private TestArrayT _testArray;

        [Button(), Horizontal("TestArray")]
        public void ReadArray()
        {
            var path = "Assets/TestArray.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testArray = TestArrayT.DeserializeFromBinary(buffer);
            }
            else
            {
                _testArray = new TestArrayT();
            }
            testArray = _testArray.Array;
        }

        [Button(), Horizontal("TestArray")]
        public void WriteArray()
        {
            var path = "Assets/TestArray.bytes";
            _testArray ??= new TestArrayT();
            _testArray.Array = testArray;
            File.WriteAllBytes(path, _testArray.SerializeToBinary());
        }

        #endregion*/

        /*#region String

        [Title("String")] public string testString;

        private TestStringT _testString;

        [Button(), Horizontal("TestString")]
        public void ReadString()
        {
            var path = "Assets/TestString.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testString = TestStringT.DeserializeFromBinary(buffer);
            }
            else
            {
                _testString = new TestStringT();
            }
            testString = _testString.Str;
        }

        [Button(), Horizontal("TestString")]
        public void WriteString()
        {
            var path = "Assets/TestString.bytes";
            _testString ??= new TestStringT();
            _testString.Str = testString;
            File.WriteAllBytes(path, _testString.SerializeToBinary());
        }

        #endregion*/

        /*#region Struc1

        [Title("Struct1")] public Vector3 testStruct1;

        private TestStruct1T _testStruct1;

        [Button(), Horizontal("TestStruct1")]
        public void ReadStruct1()
        {
            var path = "Assets/TestStruct1.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testStruct1 = TestStruct1T.DeserializeFromBinary(buffer);
            }
            else
            {
                _testStruct1 = new TestStruct1T();
            }
            testStruct1.x = _testStruct1.Position.X;
            testStruct1.y = _testStruct1.Position.Y;
            testStruct1.z = _testStruct1.Position.Z;
        }

        [Button(), Horizontal("TestStruct1")]
        public void WriteStruct1()
        {
            var path = "Assets/TestStruct1.bytes";
            _testStruct1 ??= new TestStruct1T();
            _testStruct1.Position.X = testStruct1.x;
            _testStruct1.Position.Y = testStruct1.y;
            _testStruct1.Position.Z = testStruct1.z;
            File.WriteAllBytes(path, _testStruct1.SerializeToBinary());
        }

        #endregion*/

        /*#region Struct2
        
        [Title("Struct2")] public Vector4 testStruct2;

        private TestStruct2T _testStruct2;

        [Button(), Horizontal("TestStruct2")]
        public void ReadStruct2()
        {
            var path = "Assets/TestStruct2.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testStruct2 = TestStruct2T.DeserializeFromBinary(buffer);
            }
            else
            {
                _testStruct2 = new TestStruct2T();
            }
            testStruct2.x = _testStruct2.Quaternion.Xyz.X;
            testStruct2.y = _testStruct2.Quaternion.Xyz.Y;
            testStruct2.z = _testStruct2.Quaternion.Xyz.Z;
        }

        [Button(), Horizontal("TestStruct2")]
        public void WriteStruct2()
        {
            var path = "Assets/TestStruct2.bytes";
            _testStruct2 ??= new TestStruct2T();
            _testStruct2.Quaternion.Xyz.X = testStruct2.x;
            _testStruct2.Quaternion.Xyz.Y = testStruct2.y;
            _testStruct2.Quaternion.Xyz.Z = testStruct2.z;
            _testStruct2.Quaternion.W = testStruct2.w;
            File.WriteAllBytes(path, _testStruct2.SerializeToBinary());
        }
        
        #endregion*/

        #region Union

        [Title("Union")] public num testUnionType = num.NONE;

        public int testUnionInt;

        public long testUnionLong;

        private TestUnionT _testUnion;

        [Button(), Horizontal("TestUnion")]
        public void ReadUnion()
        {
            var path = "Assets/TestUnion.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testUnion = TestUnionT.DeserializeFromBinary(buffer);
            }
            else
            {
                _testUnion = new TestUnionT() { Uni = new numUnion() };
            }
            testUnionType = _testUnion.Uni.Type;
            if (_testUnion.Uni.Type == num.bigNum)
            {
                testUnionLong = _testUnion.Uni.AsbigNum().TestLong;
            }
            else if (_testUnion.Uni.Type == num.smallNum)
            {
                testUnionInt = _testUnion.Uni.AssmallNum().TestInt;
            }
            else
            {
                testUnionLong = 0;
                testUnionInt = 0;
            }
        }

        [Button(), Horizontal("TestUnion")]
        public void WriteUnion()
        {
            var path = "Assets/TestUnion.bytes";
            _testUnion ??= new TestUnionT() { Uni = new numUnion() };
            _testUnion.Uni.Type = testUnionType;
            if (_testUnion.Uni.Type == num.smallNum)
            {
                _testUnion.Uni.Value = new num32T() { TestInt = testUnionInt };
            }
            else if (_testUnion.Uni.Type == num.bigNum)
            {
                _testUnion.Uni.Value = new num64T() { TestLong = testUnionLong }; ;
            }
            File.WriteAllBytes(path, _testUnion.Uni.Value);
        }

        #endregion

        #region Table

        [Title("Table")] 

        public bool testBool;

        public int testInt;

        public string testString;

        public Vector3 position;

        public Vector4 quaternion;

        public List<int> testArray;

        private TestTableT _testTable;
        TestTable _testTablenoT;

        [Button(), Horizontal("TestTable")]
        public void ReadTable()
        {
            var path = "Assets/TestTable.bytes";
            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                _testTable = TestTableT.DeserializeFromBinary(buffer);
            }   
            else
            {
                _testTable = new TestTableT();
            }
            testBool = _testTable.TestBool;
            testInt = _testTable.TestInt;
            testString = _testTable.TestString;
            position.x = _testTable.Position.X;
            position.y = _testTable.Position.Y;
            position.z = _testTable.Position.Z;
            quaternion.x = _testTable.Quaternion.Xyz.X;
            quaternion.y = _testTable.Quaternion.Xyz.Y;
            quaternion.z = _testTable.Quaternion.Xyz.Z;
            quaternion.w = _testTable.Quaternion.W;
            testArray = _testTable.Array.Array;
        }

        [Button(), Horizontal("TestTable")]
        public void WriteTable()
        {
            var path = "Assets/TestTable.bytes";

            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                var data = TestTable.GetRootAsTestTable(new ByteBuffer(buffer));
                data.MutateTestInt(1234);
                data.MutateTestBool(false);
                testBool = data.TestBool;
                testInt = data.TestInt;
                data.MutateIndex(TestEnum.Two);
            }

            _testTable ??= new TestTableT()
            {
                Array = new TestArrayT()
            };
            _testTable.TestBool = testBool;
            _testTable.TestInt = testInt;
            _testTable.TestString = testString;
            _testTable.Position.X = position.x;
            _testTable.Position.Y = position.y;
            _testTable.Position.Z = position.z;
            _testTable.Quaternion.Xyz.X = quaternion.x;
            _testTable.Quaternion.Xyz.Y = quaternion.y;
            _testTable.Quaternion.Xyz.Z = quaternion.z;
            _testTable.Quaternion.W = quaternion.w;
            _testTable.Array.Array = testArray;
            File.WriteAllBytes(path, _testTable.SerializeToBinary());
        }

        #endregion
    }
}