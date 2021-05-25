using System.Text;
using Base58Check;

namespace BisqAnalyser
{
    class Util
    {
        /// <summary>
        /// Utility method to convert a byte array to string containing hex-hash
        /// </summary>
        /// <param name="byteArray">Byte array to convert to hexadecimal string</param>
        /// <returns>String containing hash from byte array</returns>
        public static string ByteArrayToHex(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        /// <summary>
        /// Utility method for converting byte arrays to base58 strings based on
        /// Bitcoin protocol
        /// </summary>
        /// <param name="byteArray">Byte array to convert to base58</param>
        /// <returns>Base58 string</returns>
        public static string ByteArrayToBase58(byte[] byteArray)
        {
            
            return Base58CheckEncoding.Encode(byteArray);
        }

        /// <summary>
        /// Utility method for converting base58 strings to byte arrays based on
        /// Bitcoin protocol
        /// </summary>
        /// <param name="base58">Base58 string to convert to byte array</param>
        /// <returns>Byte array</returns>
        public static byte[] Base58ToByteArray(string base58)
        {
            return Base58CheckEncoding.Decode(base58);
        }

        /// <summary>
        /// Function used to comapre to bye arrays to check that their contents 
        /// are the same, does not handle null arrays
        /// </summary>
        /// <param name="arr1">First arry to compare</param>
        /// <param name="arr2">Second array to compare</param>
        /// <returns>Boolean representing whether or not the arrays are identical</returns>
        public static bool CompareByteArray(byte[] arr1, byte[] arr2)
        {
            if(null == arr1 || null == arr2 || arr1.Length != arr2.Length)
            {
                return false;
            }
            for(int i = 0; i < arr1.Length; ++i)
            {
                if(arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Function converts a byte array to a decimal string
        /// </summary>
        /// <param name="arr">Array to convert</param>
        /// <returns>String containing decimal values from byte array</returns>
        public static string ByteArrayToDecimalString(byte[] arr)
        {
            string output = string.Empty;
            foreach(byte b in arr)
            {
                output += b.ToString();
            }
            return output;
        }

        /// <summary>
        /// Function used to prepend a byte value to a byte array, 
        /// AKA, insert a new element before element 0
        /// </summary>
        /// <param name="arr">Array to prepend onto</param>
        /// <param name="val">Value to prepend onto array</param>
        /// <returns>Byte array with prepended byte value</returns>
        public static byte[] PrependByteVal(byte[] arr, byte val)
        {

            byte[] newArr = new byte[arr.Length + 1];
            for(int i = 0; i < arr.Length; ++i)
            {
                newArr[i + 1] = arr[i];
            }
            newArr[0] = val;
            return newArr;
        }
    }
}
