﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SecretNest.Hardware.Text
{
    public class TextRenderer
    {
        private readonly Graphics _targetGraphics;
        private readonly Size _charSize;
        private readonly TextRendererSource[] _sources;
        private readonly Rectangle[] _targetRectangles;

        public int StartX { get; }
        public int StartY { get; }
        public Size CharSize => _charSize;
        public int CharCount { get; }
        public Graphics TargetGraphics => _targetGraphics;

        public TextRenderer(Graphics targetGraphics, int startX, int startY, Size charSize, int charCount, params TextRendererSource[] sources)
        {
            _targetGraphics = targetGraphics;
            _charSize = charSize;
            _sources = sources;
            StartX = startX;
            StartY = startY;
            _targetRectangles = new Rectangle[charCount];
            for (int i = 0; i < charCount; i++)
            {
                _targetRectangles[i] = new Rectangle(startX, startY, charSize.Width, charSize.Height);
                startX += charSize.Width;
            }
        }

#if NETSTANDARD2_1
        public void Render(string text, ReadOnlySpan<byte> colorIndices)
        {
            int index = 0;
            foreach (var c in text)
            {
                var source = _sources[colorIndices[index]];
                if (!source.Points.TryGetValue(c, out var point))
                {
                    point = source.Points['?'];
                }
                _targetGraphics.DrawImage(source.Image, _targetRectangles[index], 
                    point.X, point.Y, _charSize.Width, _charSize.Height, GraphicsUnit.Pixel);

                index++;
            }
        }
#endif

        public void Render(string text, byte[] colorIndices)
        {
            int index = 0;
            foreach (var c in text)
            {
                var source = _sources[colorIndices[index]];
                if (!source.Points.TryGetValue(c, out var point))
                {
                    point = source.Points['?'];
                }
                _targetGraphics.DrawImage(source.Image, _targetRectangles[index], 
                    point.X, point.Y, _charSize.Width, _charSize.Height, GraphicsUnit.Pixel);

                index++;
            }
        }

        public void Render(string text, Func<int, int> sourceIndexSelector)
        {
            int index = 0;
            foreach (var c in text)
            {
                var source = _sources[sourceIndexSelector(index)];
                if (!source.Points.TryGetValue(c, out var point))
                {
                    point = source.Points['?'];
                }
                _targetGraphics.DrawImage(source.Image, _targetRectangles[index], 
                    point.X, point.Y, _charSize.Width, _charSize.Height, GraphicsUnit.Pixel);

                index++;
            }
        }
    }

    public class TextRendererSource
    {
        public TextRendererSource(Bitmap image, IReadOnlyDictionary<char, Point> points)
        {
            Image = image;
            Points = points;
        }

        public Bitmap Image { get; }
        public IReadOnlyDictionary<char, Point> Points { get; }
    }
}
