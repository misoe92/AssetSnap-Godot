// MIT License

// Copyright (c) 2024 Mike Sørensen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using Godot;

namespace AssetSnap.Helpers
{
	public static class UniqueHelper
	{
		private const int RandomLength = 12;
	
		public static string GenerateId()
		{
			// Generate a random string
			string randomPart = GenerateRandomString(RandomLength);

			// Concatenate timestamp and random string to form the unique ID
			string uniqueId = randomPart.Split(" ").Join("");

			return uniqueId;
		}

		private static string GenerateRandomString(int length)
		{
			// Define characters used for random string generation (only letters)
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

			// Create a random number generator
			Random random = new Random();
			
			// Generate a random string of specified length
			char[] randomString = new char[length];
			for (int i = 0; i < length; i++)
			{
				char rs = chars[random.Next(chars.Length)];
				randomString[i] = rs;
			}

			return new string(randomString);
		}
	}
}