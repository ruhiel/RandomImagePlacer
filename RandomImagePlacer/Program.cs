using CommandLine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace RandomImagePlacer
{
    class Program
    {
        static void Main(string[] args) => Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

        static void RunOptions(Options opts)
        {
            try
            {
                GenerateRandomImage(opts);
                Console.WriteLine("\n処理が正常に完了しました。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラーが発生しました: {ex.Message}");
            }
        }

        static void HandleParseError(IEnumerable<Error> errs) => Console.WriteLine("引数の指定が正しくありません。--help を参照してください。");

        static void GenerateRandomImage(Options opts)
        {
            if (!Directory.Exists(opts.InputPath))
            {
                throw new DirectoryNotFoundException($"フォルダが見つかりません: {opts.InputPath}");
            }

            string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };
            var files = Directory.GetFiles(opts.InputPath)
                                 .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                 .ToArray();

            if (files.Length == 0)
            {
                throw new Exception("画像ファイルがフォルダ内にありません。");
            }

            var rand = new MersenneTwister(Environment.TickCount);
            var placedRects = new List<Rectangle>();

            using (var canvas = new Bitmap(opts.Width, opts.Height))
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                var successCount = 0;
                for (var i = 0; i < opts.Count; i++)
                {
                    var randomFile = files[rand.Next(0, files.Length)];

                    using (var img = Image.FromFile(randomFile))
                    {
                        var destWidth = (int)(img.Width * opts.ResizeFactor);
                        var destHeight = (int)(img.Height * opts.ResizeFactor);

                        var placed = false;
                        var attempts = 0;
                        const int maxAttempts = 200;

                        while (!placed && attempts < maxAttempts)
                        {
                            attempts++;
                            var x = rand.Next(0, opts.Width - destWidth);
                            var y = rand.Next(0, opts.Height - destHeight);

                            var newRect = new Rectangle(x, y, destWidth, destHeight);

                            // 衝突判定（簡易的に矩形で判定）
                            if (!placedRects.Any(r => r.IntersectsWith(newRect)))
                            {
                                // ランダムな角度を決定 (-MaxAngle ～ MaxAngle)
                                float angle = 0;
                                if (opts.MaxAngle > 0)
                                {
                                    angle = (float)(rand.NextDouble() * 2 * opts.MaxAngle - opts.MaxAngle);
                                }

                                // グラフィックス状態の保存
                                var state = g.Save();

                                // 回転の中心を画像の中心に設定
                                var centerX = x + destWidth / 2f;
                                var centerY = y + destHeight / 2f;

                                g.TranslateTransform(centerX, centerY);
                                g.RotateTransform(angle);

                                // 画像の描画（中心に移動しているため、座標は中心からのオフセット）
                                g.DrawImage(img, -destWidth / 2f, -destHeight / 2f, destWidth, destHeight);

                                // 状態の復元
                                g.Restore(state);

                                placedRects.Add(newRect);
                                placed = true;
                                successCount++;
                            }
                        }
                    }
                }
                canvas.Save(opts.OutputPath, ImageFormat.Png);
                Console.WriteLine($"{successCount} 個の画像を配置しました（指定数: {opts.Count}）");
            }
        }
    }
}