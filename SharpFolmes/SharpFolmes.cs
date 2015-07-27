namespace SharpFolmes
{
    public static class ByteConverter
    {
        public static unsafe void GetBytes(long value, byte[] bytes, int byteIndex)
        {
            fixed (byte* numPtr = &bytes[byteIndex])
                *(long*)numPtr = value;
        }

        public static unsafe void GetBytes(short value, byte[] bytes, int byteIndex)
        {
            fixed (byte* numPtr = &bytes[byteIndex])
                *(short*)numPtr = value;
        }

        public static unsafe long ToInt64(byte[] value, int startIndex)
        {
            fixed (byte* numPtr = &value[startIndex])
                return *((long*)numPtr);
        }

        public static unsafe int ToInt32(byte[] value, int startIndex)
        {
            fixed (byte* numPtr = &value[startIndex])
                return *((int*)numPtr);
        }

        public static unsafe short ToInt16(byte[] value, int startIndex)
        {
            fixed (byte* numPtr = &value[startIndex])
                return *((short*)numPtr);
        }
    }
}
