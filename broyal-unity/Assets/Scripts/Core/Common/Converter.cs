using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Scripts.Core.Common
{
	public static class Converter
	{
		public static Color StringToColor( string s )
		{
			if ( s == "" ) return Color.gray;

			int r = 0xff;
			int g = 0xff;
			int b = 0xff;
			int colorInt = 0xffffff;
			colorInt = Convert.ToInt32( s, 16 );
			//if ( int.TryParse( s, global::System.Globalization.NumberStyles.HexNumber, out colorInt ) )
			{
				r = ( colorInt >> 16 ) & 255;
				g = ( colorInt >> 8 ) & 255;
				b = colorInt & 255;
			}

			return new Color( r / 255f, g / 255f, b / 255f );
		}

		public static string SecondsToTimeString(int seconds)
		{
			int minutes = seconds / 60;
			int last_seconds = seconds % 60;
			
			return string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", last_seconds);
		}
		
		public static string Md5Sum(string strToEncrypt)
		{
			UTF8Encoding ue = new UTF8Encoding();
			byte[] bytes = ue.GetBytes(strToEncrypt);
		 
			// encrypt bytes
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);
		 
			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";
		 
			for (int i = 0; i < hashBytes.Length; i++)
			{
				hashString += Convert.ToString( hashBytes[ i ], 16 ).PadLeft( 2, '0' );
			}
		 
			return hashString.PadLeft(32, '0');
		}

		public static byte[] StringToBytesArray(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
		
		public static string BytesArrayToString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
	}
}

