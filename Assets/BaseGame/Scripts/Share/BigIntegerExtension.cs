using System.Numerics;

public static class BigIntegerExtension
{
    private static string[] SuffixArray { get; set; } = new string[] 
        { 
            "", 
            "K", 
            "M", 
            "B", 
            "T", 
            "Q", 
            "QQ", 
            "S", 
            "SS", 
            "O", 
            "N", 
            "D", 
            "UN", 
            "DD", 
            "TR", 
            "QT", 
            "QN", 
            "SD", 
            "SSD", 
            "OD", 
            "ND", 
            "VG", 
            "UVG", 
            "DVG", 
            "TVG", 
            "QTVG", 
            "QNVG", 
            "SP", 
            "SSP", 
            "OSP", 
            "NSP", 
            "OVG" }; 
    
    public static string ToSuffixString(this BigInteger value)
    {
        int index = 0;
        BigInteger tempValue = value;
        while (tempValue >= 1000)
        {
            tempValue /= 1000;
            index++;
        }
        return $"{tempValue}{SuffixArray[index]}";
    }
}