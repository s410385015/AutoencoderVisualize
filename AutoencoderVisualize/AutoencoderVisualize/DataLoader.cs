using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoencoderVisualize
{
    static class DataLoader
    {
        public static List<String> ReadTags(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath),System.Text.Encoding.Default);
            List<String> searchList = new List<String>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                searchList.Add(line);
            }
            return searchList;
        }

        public static List<List<double>> ReadData(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath), System.Text.Encoding.Default);
            List<List<double>> searchList = new List<List<double>>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                List<double> result = (line.Split(',')).Select(x => double.Parse(x)).ToList();

                searchList.Add(result);
            }
            return searchList;
        }

        public static List<List<float>> ReadDataFloat(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath), System.Text.Encoding.Default);
            List<List<float>> searchList = new List<List<float>>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                List<float> result = (line.Split(',')).Select(x => float.Parse(x)).ToList();

                searchList.Add(result);
            }
            return searchList;
        }
    }
}
