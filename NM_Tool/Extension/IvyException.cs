using NM_Tool.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NM_Tool.Extension
{
    /// <summary>
    /// 异常信息处理
    /// </summary>
    public static class IvyException
    {

        public static string GetMessage(this Exception ex)
        {
            exception = ex;
            Log.writeLog(exception);
            return Message;
        }

        public static void ShowMessage(this Exception ex)
        {
            exception = ex;
            Log.writeLog(exception);
        }

        private static Exception exception;
        private static string _Message;
        public static string Message
        {
            get
            {
                Type type = typeof(System.Exception);
                Exception e = exception;
                if (exception.InnerException != null)
                {
                    e = exception.InnerException;
                    type = exception.InnerException.GetType();
                }
                else if (exception != null)
                {
                    e = exception;
                    type = exception.GetType();
                }
                switch (type.BaseType.FullName)
                {
                    //由System.SystemException派生的异常类型
                    case "System.SystemException":
                        GetSystemException(type);
                        break;
                    //由System.ArgumentException派生的异常类型
                    case "System.ArgumentException":
                        GetArgumentException(type);
                        break;
                    //由System.ArithmeticException派生的异常类型
                    case "System.ArithmeticException":
                        GetArithmeticException(type);
                        break;
                    //由System.IOException派生的异常类型
                    case "System.IOException":
                        GetIOException(type);
                        break;
                    case "System.TypeLoadException":
                        GetTypeLoadException(type);
                        break;
                    case "System.MethodAccessException":
                        GetMethodAccessException(type);
                        break;
                    case "System.MemberAccessException":
                        GetMemberAccessException(type);
                        break;
                    case "System.NotSupportedException":
                        GetNotSupportedException(type);
                        break;

                    //数据库错误
                    case "System.Data.SqlClient.SqlException":
                        //主键约束问题
                        if (e.Message.Contains("违反了") && e.Message.Contains("PRIMARY KEY") && e.Message.Contains("约束"))
                        {
                            Match m = Regex.Match(e.Message, @"'dbo.([\s\S]*?)'");
                            _Message = "不可以在表:" + m.Result("$1") + "中插入重复值";
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    default:
                        if (exception.InnerException != null)
                            _Message = exception.InnerException.Message;
                        else
                            _Message = exception.Message;
                        break;

                }
                return _Message;
            }
        }


        private static void GetSystemException(Type type)
        {
            switch (type.FullName)
            {
                case "System.AccessViolationException":
                    _Message = "在试图读写受保护内存时引发的异常";
                    break;
                case "System.ArgumentException":
                    _Message = "在向方法提供的其中一个参数无效时引发的异常";
                    break;
                case "System.Collections.Generic.KeyNotFoundException":
                    _Message = "指定用于访问集合中元素的键与集合中的任何键都不匹配时所引发的异常";
                    break;
                case "System.IndexOutOfRangeException":
                    _Message = "访问数组时，因元素索引超出数组边界而引发的异常";
                    break;
                case "System.InvalidCastException":
                    _Message = "因无效类型转换或显示转换引发的异常";
                    break;
                case "System.InvalidOperationException":
                    _Message = "当方法调用对于对象的当前状态无效时引发的异常";
                    break;
                case "System.InvalidProgramException":
                    _Message = "当程序包含无效Microsoft中间语言（MSIL）或元数据时引发的异常，这通常表示生成程序的编译器中有bug";
                    break;
                case "System.IO.IOException":
                    _Message = "发生I/O错误时引发的异常";
                    break;
                case "System.NotImplementedException":
                    _Message = "在无法实现请求的方法或操作时引发的异常";
                    break;
                case "System.NullReferenceException":
                    _Message = "尝试对空对象引用进行操作时引发的异常";
                    break;
                case "System.OutOfMemoryException":
                    _Message = "没有足够的内存继续执行程序时引发的异常";
                    break;
                case "System.StackOverflowException":
                    _Message = "挂起的方法调用过多而导致执行堆栈溢出时引发的异常";
                    break;
                case "System.ArrayTypeMismatchException":
                    _Message = "试图在数组中存储错误类型的对象";
                    break;
                case "System.BadImageFormatException":
                    _Message = "图形的格式错误";
                    break;
                case "System.FormatException":
                    _Message = "参数格式错误";
                    break;
                case "System.NotSupportedException":
                    _Message = "调用的方法在类中没有实现";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetArgumentException(Type type)
        {
            switch (type.FullName)
            {
                case "System.ArgumentNullException":
                    _Message = "当将空引用传递给不接受它作为有效参数的方法时引发的异常";
                    break;
                case "System.ArgumentOutOfRangeException":
                    _Message = "当参数值超出调用的方法所定义的允许取值范围时引发的异常";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetArithmeticException(Type type)
        {
            switch (type.FullName)
            {
                case "System.DivideByZeroException":
                    _Message = "试图用零除整数值或十进制数值时引发的异常";
                    break;
                case "System.NotFiniteNumberException":
                    _Message = "当浮点值为正无穷大、负无穷大或非数字（NaN）时引发的异常";
                    break;
                case "System.OverflowException":
                    _Message = "在选中的上下文中所进行的算数运算、类型转换或转换操作导致溢出时引发的异常";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetIOException(Type type)
        {
            switch (type.FullName)
            {
                case "System.IO.DirectoryNotFoundException":
                    _Message = "当找不到文件或目录的一部分时所引发的异常";
                    break;
                case "System.IO.DriveNotFoundException":
                    _Message = "当尝试访问的驱动器或共享不可用时引发的异常";
                    break;
                case "System.IO.EndOfStreamException":
                    _Message = "读操作试图超出流的末尾时引发的异常";
                    break;
                case "System.IO.FileLoadException":
                    _Message = "当找到托管程序却不能加载它时引发的异常";
                    break;
                case "System.IO.FileNotFoundException":
                    _Message = "试图访问磁盘上不存在的文件失败时引发的异常";
                    break;
                case "System.IO.PathTooLongException":
                    _Message = "当路径名或文件名超过系统定义的最大长度时引发的异常";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetTypeLoadException(Type type)
        {
            switch (type.FullName)
            {
                case "System.DllNotFoundException":
                    _Message = "找不到引用的dll";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetMethodAccessException(Type type)
        {
            switch (type.FullName)
            {
                case "System.MethodAccessException":
                    _Message = "试图访问私有或者受保护的方法";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetMemberAccessException(Type type)
        {
            switch (type.FullName)
            {
                case "MissingMemberException":
                    _Message = "访问一个无效版本的dll";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void GetNotSupportedException(Type type)
        {
            switch (type.FullName)
            {
                case "PlatformNotSupportedException":
                    _Message = "平台不支持某个特定属性时抛出该错误";
                    break;
                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }

        private static void Get(Type type)
        {
            switch (type.FullName)
            {

                default:
                    if (exception.InnerException != null)
                        _Message = exception.InnerException.Message;
                    else
                        _Message = exception.Message;
                    break;
            }
        }


    }
}
