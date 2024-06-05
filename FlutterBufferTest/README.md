# FlatBuffers

<font size="6">

[课程视频在此](http://192.168.0.250/个人专用/G-苟谆/FlatBuffers.mp4)

---

相关链接：

[Lightning Talk: FlatBuffers](https://www.youtube.com/watch?v=olmL1fUnQAQ)

[FlatBuffers官网](https://google.github.io/flatbuffers/)

[深入浅出 FlatBuffers 之 Schema](https://halfrost.com/flatbuffers_schema/)

[FlatBuffers详解](https://blog.csdn.net/linuxheik/article/details/77835663)

[深入浅出FlatBuffers原理](https://baijiahao.baidu.com/s?id=1705686535856873496&wfr=spider&for=pc)

---

序列化快的大致原理：

将数据直接存入一个二进制字节数组中，每个元素能基本分为两个部分，头存放一个可以帮助找到实际数据的信息，尾存放实际数据，这样在实际读取的时候，当读到头的时候，就能直接寻址读到实际数据

对于8位~64位的整数以及浮点这种标量，以及struct，头部只需要存放一个偏移信息就好

struct因为目前只能存储标量和内嵌struct，所以也支持连续存储和连续寻址

对于数组和字符串这类矢量而言，它们增加了一个部分来存储长度，字符串按照UTF-8的方式编码，最后一位还会额外写入null结束字符

对于table而言，table多了一个描述性的vtable，这个vtable在一定情况下是可以多个table公用的，table会记录使用哪一个vtable来解析自己，vtable中可以直接记录标量或struct的实际数据。如果是矢量或者table，则存放一个相对的偏移信息，再进行一次偏移读取

所有数据在Schema中就已经定义好了，因此在实际取出二进制数据后，能够直接转化为对应的类型

---

前后兼容的大致原理：

目前仅table可以实现前后兼容，原因正是因为它多出来的vtable，当一个新的Schema中，某个字段被弃用时，新的vtable就标记跳过这个字段的区间(所以不能把字段删掉，只能标记为(deprecated))，如果是新增的字段，会有默认值，默认值不会存入实际数据中，所以读取老数据也无影响

默认值猜测是依赖于在编译出的接口代码中，显示声明出字段的默认值，从而实现不存入数据

---

Schema支持书写的类型:

byte, short, int, long

float, double

bool

enum

struct

vector

string

union

table

root_type

---

注意事项：

对于目前我们正早使用的这套FlatBuffers工具，即使有很多个fbs文件，也只支持一个root_type，多了会递归生成卡住

struct内部只支持标量和struct，不能往里面放string和vector等复杂类型

如果要使用union，因为union会将类型转变为类似枚举进行存放，且仅8位，所以内部包含的类型不能超过256个

如果要修改schema，增加字段一定要在最后增加，删除字段只能加(deprecated)，因为在读取数据时是连续读取的

table名字和字段名称的修改最好经过验证后再执行

因为可能需要将数据打进ab包，请选择Unity能打进ab包的格式来存放二进制数据，比如bytes

---

C#侧使用

我们重写了.fbs后缀文件的ScriptedImporter，使.fbs文件可以被Unity识别，它有两个选项可以选择

+ Is Mutable
+ Is Object API

两者分别对应Schema编译器的两个选项

第一个对应生成结构体中的Mutate开头的方法，可以让我们直接对FlatBuffers内存态的指定数据进行修改，不过貌似目前只支持标量和数组，字符串也不支持

第二个对应生成类中后缀带"T"的类，它的结构和Schema的定义相同，对应字段为使用属性访问，里面有从二进制到对象，对象到二进制的封装方法，这个API更方便，但也伴随着对象开销

在操作数据时，需要注意，写入时，顺序是从后往前写，在读取时，顺序是从前往后写

只有标量能够直接被Add进一个builder，字符串需要调用builder的CreateString方法，table和struct则需要调用自己解析类中的Pack方法

可以对能成为主键的字段打上(key)，这样在用这些table作为数组元素时，生成的C#代码会包含一个使用Key查找对应元素的方法

---

写在最后：

Json在大批量数据时劣势极度明显，文本的字符串存储方式，文本的符号化读写，数据的解析，都是很费时间的，唯一好点的可能就是可读性了把，但一旦数据量上去了，打开文本查看都费劲儿，也谈不上什么可读性了

如果在项目初期大家能考虑到，数据量的大小是否能够容忍Json来驾驭，然后选择二进制数据的方式的话，会少走一些优化方向的弯路

如果后期发现性能不行了，再去修改数据储存格式的话，也只能尽量拜托最熟悉自己项目数据结构的大家来重写它，包括旧数据的转换，读写类型的支持，新数据的应用等，这些都是比较花精力的东西

FlatBuffer不仅性能好，也能提前帮助大家仔细梳理自己项目的数据结构，虽然理Schema会花点时间，但俗话说，长痛不如短痛，还是希望大家能够将它使用起来

</font>