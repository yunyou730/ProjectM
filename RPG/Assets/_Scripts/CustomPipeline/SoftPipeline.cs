using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace crayon
{
    public class SoftPipeline : MonoBehaviour
    {
        private Canvas canvas = null;
        private Color clearColor = Color.black;
        private int width = 400;
        private int height = 300;
        private void Start()
        {
            canvas = new Canvas();
            canvas.Init(width,height);
            canvas.Clear(clearColor);
            
            GetComponent<Renderer>().material.mainTexture = canvas.GetTexture();
            
            canvas.SetDrawColor(Color.red);
            
            /*
            canvas.DrawPixel(0,0);
            canvas.DrawPixel(width - 1,0);
            canvas.DrawPixel(0,height - 1);
            canvas.DrawPixel(width - 1,height - 1);
            */
            //canvas.DrawLine(0,0,100,80);
            //canvas.DrawLine(100,80,0,80);
            
            canvas.DrawLine(100,80,0,100);
            canvas.DrawLine(10,10,20,100);
            canvas.DrawLine(20,100,10,10);
            canvas.DrawLine(0,0,200,150);
            
            canvas.DrawLine(10,50,200,130);
            
            //canvas.DrawLine(0,0,30,100);
            canvas.Present();
        }
        
        
        private void Update()
        {
            /*
            canvas.Clear(clearColor);
            DoDraw();
            canvas.Present();
            */
        }

        private void DoDraw()
        {
            
        }
    }
}
