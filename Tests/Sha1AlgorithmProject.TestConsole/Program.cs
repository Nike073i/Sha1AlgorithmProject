﻿using Sha1AlgorithmProject.Math;
using System.Text;

namespace Sha1AlgorithmProject.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var message = "test";
            //var message = "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss";
            //var message = "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss";
            var listBytes = Encoding.ASCII.GetBytes(message).ToList();
            Sha1.CompleteBlock(ref listBytes);
            Sha1.BlockPrepare(listBytes);
        }
    }
}

