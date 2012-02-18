﻿using System;
using System.Text;
using NUnit.Framework;

namespace Logos.Utility.Tests
{
	[TestFixture]
	public class Ascii85Tests
	{
		[Test]
		public void WikipediaSample()
		{
			const string c_text = "Man is distinguished, not only by his reason, but by this singular passion from other animals, which is a lust of the mind, that by a perseverance of delight in the continued and indefatigable generation of knowledge, exceeds the short vehemence of any carnal pleasure.";
			const string c_encoded = @"9jqo^BlbD-BleB1DJ+*+F(f,q/0JhKF<GL>Cj@.4Gp$d7F!,L7@<6@)/0JDEF<G%<+EV:2F!,O<DJ+*.@<*K0@<6L(Df-\0Ec5e;DffZ(EZee.Bl.9pF""AGXBPCsi+DGm>@3BB/F*&OCAfu2/AKYi(DIb:@FD,*)+C]U=@3BN#EcYf8ATD3s@q?d$AftVqCh[NqF<G:8+EV:.+Cf>-FD5W8ARlolDIal(DId<j@<?3r@:F%a+D58'ATD4$Bl@l3De:,-DJs`8ARoFb/0JMK@qB4^F!,R<AKZ&-DfTqBG%G>uD.RTpAKYo'+CT/5+Cei#DII?(E,9)oF*2M7/c";
			byte[] bytes = Encoding.ASCII.GetBytes(c_text);

			Assert.AreEqual(c_encoded, Ascii85.Encode(bytes));
			CollectionAssert.AreEqual(bytes, Ascii85.Decode(c_encoded));
		}

		[TestCase(new byte[0], "")]
		[TestCase(new byte[] { 0, 0, 0, 0 }, "z")]
		[TestCase(new byte[] { 0 }, "!!")]
		[TestCase(new byte[] { 1 }, "!<")]
		[TestCase(new byte[] { 1, 1 }, "!<E")]
		[TestCase(new byte[] { 1, 1, 1 }, "!<E3")]
		[TestCase(new byte[] { 1, 1, 1, 1 }, "!<E3%")]
		[TestCase(new byte[] { 10 }, "$3")]
		[TestCase(new byte[] { 10, 10 }, "$46")]
		[TestCase(new byte[] { 10, 10, 10 }, "$47+")]
		[TestCase(new byte[] { 10, 10, 10, 10 }, "$47+I")]
		[TestCase(new byte[] { 100 }, "A,")]
		[TestCase(new byte[] { 100, 100 }, "A7P")]
		[TestCase(new byte[] { 100, 100, 100 }, "A7T3")]
		[TestCase(new byte[] { 100, 100, 100, 100 }, "A7T4]")]
		[TestCase(new byte[] { 255, 255, 255, 255 }, "s8W-!")]
		public void RoundTrip(byte[] bytes, string encoded)
		{
			Assert.That(Ascii85.Encode(bytes), Is.EqualTo(encoded));
			Assert.That(Ascii85.Decode(encoded), Is.EqualTo(bytes));
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void EncodeNull()
		{
			Ascii85.Encode(null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void DecodeNull()
		{
			Ascii85.Decode(null);
		}
		
		[Test, ExpectedException(typeof(FormatException))]
		public void ZInBlock()
		{
			Ascii85.Decode("abzde");
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void InvalidCharacterInBlock()
		{
			Ascii85.Decode("rstuv");
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void ShortFinalBlock()
		{
			Ascii85.Decode("a");
		}

		[ExpectedException(typeof(FormatException))]
		[TestCase("uuuuu")]
		[TestCase("s8W-<")]
		[TestCase("s8")]
		public void DecodeOverflow(string encoded)
		{
			Ascii85.Decode(encoded);
		}
	}
}
