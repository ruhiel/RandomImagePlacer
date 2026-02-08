using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomImagePlacer
{
    class Options
    {
        [Option('w', "width", Required = true, HelpText = "出力画像の幅(px)")]
        public int Width { get; set; }

        [Option('h', "height", Required = true, HelpText = "出力画像の高さ(px)")]
        public int Height { get; set; }

        [Option('c', "count", Required = true, HelpText = "貼り付ける画像の数")]
        public int Count { get; set; }

        [Option('i', "input", Required = true, HelpText = "入力画像フォルダのパス")]
        public string InputPath { get; set; }

        [Option('o', "output", Default = "result.png", HelpText = "出力ファイル名")]
        public string OutputPath { get; set; }

        [Option('f', "factor", Default = 0.5f, HelpText = "縮小倍率 (0.1 - 1.0)")]
        public float ResizeFactor { get; set; }
    }

}
