using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Cryptography;
using UnityEngine;

namespace crayon
{
    public class DDA
    {
        public static void Draw(int x1,int y1,int x2,int y2,List<Vector2> result)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            if (Math.Abs(dy) < Math.Abs(dx))
            {
                if (x1 == x2)
                {
                    if (y2 < y1)
                    {
                        Util.Exchange(ref y1,ref y2);
                    }
                    for (int y = y1;y <= y2;y++)
                    {
                        result.Add(new Vector2(x1,y));
                    }
                }
                else
                {
                    float k = dy / dx;
                    if (x2 < x1)
                    {
                        Util.Exchange(ref x1,ref x2);
                        Util.Exchange(ref y1,ref y2);
                    }
                    
                    int x;
                    float y;
                    for (x = x1,y = y1;x <= x2;x++)
                    {
                        int drawX = x;
                        int drawY = (int) (y + 0.5f);
                        result.Add(new Vector2(drawX,drawY));
                        y += k;
                    }
                }
            }
            else
            {
                if (y1 == y2)
                {
                    if (x2 < x1)
                    {
                        Util.Exchange(ref x1,ref x2);
                    }
                    for (int x = x1;x <= x2;x++)
                    {
                        result.Add(new Vector2(x,y1));
                    }
                }
                else
                {
                    float k = dx / dy;
                    if (y2 < y1)
                    {
                        Util.Exchange(ref x1,ref x2);
                        Util.Exchange(ref y1,ref y2);
                    }

                    int y;
                    float x;
                    for(y = y1,x = x1;y <= y2;y++)
                    {
                        int drawY = y;
                        int drawX = (int) (x + 0.5f);
                        result.Add(new Vector2(drawX,drawY));
                        x += k;
                    }
                }
                
            }
        }
    }
}