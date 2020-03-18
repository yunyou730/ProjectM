using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace crayon
{
    public class Canvas
    {
        private Texture2D texture = null;
        private int width, height;
        private Color drawColor;
        
        public void Init(int width,int height)
        {
            this.width = width;
            this.height = height;
            texture = new Texture2D(width,height);
        }
        
        public void Clear(Color clearColor)
        {
            for (int row = 0;row < height;row++)
            {
                for (int col = 0;col < width;col++)
                {
                    texture.SetPixel(col, row, clearColor);
                }
            }
        }

        public void DrawPixel(int x,int y)
        {
            texture.SetPixel(x,y, drawColor);
        }

        public void DrawPixels(List<Vector2> points)
        {
            foreach (Vector2 point in points)
            {
                DrawPixel((int)point.x, (int)point.y);
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            List<Vector2> points = new List<Vector2>();
            DDA.Draw(x1, y1, x2, y2, points);
            DrawPixels(points);
        }

        public void Present()
        {
            texture.Apply();            
        }

        public Texture2D GetTexture()
        {
            return texture;
        }

        public void SetDrawColor(Color drawColor)
        {
            this.drawColor = drawColor;
        }
    }
}
