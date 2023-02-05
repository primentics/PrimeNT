namespace AzyWorks.IO.Binary
{
    public enum BinaryNextFlag : byte
    {
        NextJsonValue = 0x000,
        NextUnknownValue = 0x005,
        NextNullValue = 0x010,
        NextBinaryObjectValue = 0x015,
        NextBinaryValue = 0x020,
        NextConvertibleValue = 0x025,
        NextStringValue = 0x030,
        NextArrayValue = 0x035,
        NextEnumerableValue = 0x040,
        NextDictionaryValue = 0x045,
        NextEnumValue = 0x050,
        NextTimeSpanValue = 0x055,
        NextDateTimeValue = 0x060
    }
}
