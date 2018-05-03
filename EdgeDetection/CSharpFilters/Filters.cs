using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters; 
using System.Drawing.Drawing2D;  
using System.Collections.Generic;

using AForge.Imaging.Textures;

namespace CSharpFilters
{
    public class ConvMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;
        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
    public class ConvMatrix5X5
    {
        public int r00 = 0, r01 = 0, r02 = 0, r03 = 0, r04 = 0;
        public int r10 = 0, r11 = 0, r12 = 0, r13 = 0, r14 = 0;     //Pixel = 1, MidRight = 0;
        public int r20 = 0, r21 = 0, Pixel5 = 1, r23 = 0, r24 = 0;
        public int r30 = 0, r31 = 0, r32 = 0, r33 = 0, r34 = 0;
        public int r40 = 0, r41 = 0, r42 = 0, r43 = 0, r44 = 0;  // BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;
        public void SetAll(int nVal)
        {
            r00 = r01 = r02 = r03 = r04 = nVal;
            r10 = r11 = r12 = r13 = r14 = nVal;
            r20 = r21 = Pixel5 = r23 = r24 = nVal;
            r30 = r31 = r32 = r33 = r34 = nVal;
            r40 = r41 = r42 = r43 = r44 = nVal;  // BottomMid = 0, BottomRight = 0;

        }
    }
    public class ConvMatrix7X7
    {
        public int r00 = 0, r01 = 0, r02 = 0, r03 = 0, r04 = 0, r05 = 0, r06 = 0;
        public int r10 = 0, r11 = 0, r12 = 0, r13 = 0, r14 = 0, r15 = 0, r16 = 0;
        public int r20 = 0, r21 = 0, r22 = 0, r23 = 0, r24 = 0, r25 = 0, r26 = 0;
        public int r30 = 0, r31 = 0, r32 = 0, Pexil7 = 0, r34 = 0, r35 = 0, r36 = 0;
        public int r40 = 0, r41 = 0, r42 = 0, r43 = 0, r44 = 0, r45 = 0, r46 = 0;
        public int r50 = 0, r51 = 0, r52 = 0, r53 = 0, r54 = 0, r55 = 0, r56 = 0;
        public int r60 = 0, r61 = 0, r62 = 0, r63 = 0, r64 = 0, r65 = 0, r66 = 0;
        public int Factor = 1;
        public int Offset = 0;
        public void SetAll(int nVal)
        {
            r00 = r01 = r02 = r03 = r04 = r05 = r06 = nVal;
            r10 = r11 = r12 = r13 = r14 = r15 = r16 = nVal;
            r20 = r21 = r22 = r23 = r24 = r25 = r26 = nVal;
            r30 = r31 = r32 = Pexil7 = r34 = r35 = r36 = nVal;
            r40 = r41 = r42 = r43 = r44 = r45 = r46 = nVal;
            r50 = r51 = r52 = r53 = r54 = r55 = r56 = nVal;
            r60 = r61 = r62 = r63 = r64 = r65 = r66 = nVal;
        }
    }
    public class BitmapFilter
    {
        public const short EDGE_DETECT_SOBEL3X3 = 0;
        public const short EDGE_DETECT_CANNY3X3 = 1;
        public const short EDGE_DETECT_SOBEL5X5 = 2;
        public const short EDGE_DETECT_SOBEL7X7 = 3;
        public const short EDGE_DETECT_CANNY5X5 = 4;
        public const short EDGE_DETECT_CANNY7X7 = 5;



        public static bool Conv3x3(Bitmap b, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (m.Factor == 0) return false;

            Bitmap bSrc = (Bitmap)b.Clone();

            // GDI+  -  returns format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = (stride * 2);



            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;
                int sumr0 = 0, sumr1 = 0, sumr2 = 0;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {

                        sumr0 = (pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight);
                        sumr1 = (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight);
                        sumr2 = (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight);
                        nPixel = (((sumr0 + sumr1 + sumr2) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;


                        sumr0 = (pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight);
                        sumr1 = (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight);
                        sumr2 = (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight);
                        nPixel = (((sumr0 + sumr1 + sumr2) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[4 + stride] = (byte)nPixel;


                        sumr0 = (pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight);
                        sumr1 = (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight);
                        sumr2 = (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight);
                        nPixel = (((sumr0 + sumr1 + sumr2) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return true;
        }

        public static bool Conv5x5(Bitmap b, ConvMatrix5X5 m)
        {
            // Avoid divide by zero errors
            if (m.Factor == 0) return false;

            Bitmap bSrc = (Bitmap)b.Clone();

            // GDI+  -  returns format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            int stride3 = stride * 3;
            int stride4 = stride * 4;


            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride - b.Width * 3;

                int nWidth = b.Width - 5;
                int nHeight = b.Height - 5;

                int nPixel = 0;
                int sumr0 = 0, sumr1 = 0, sumr2 = 0, sumr3 = 0, sumr4 = 0;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        // RED COLOR HERE 2 
                        sumr0 = (pSrc[2] * m.r00) + (pSrc[5] * m.r01) + (pSrc[8] * m.r02) + (pSrc[11] * m.r03) + (pSrc[14] * m.r04);
                        sumr1 = (pSrc[2 + stride] * m.r10) + (pSrc[5 + stride] * m.r11) + (pSrc[8 + stride] * m.r12) + (pSrc[11 + stride] * m.r13) + (pSrc[14 + stride] * m.r14);
                        sumr2 = (pSrc[2 + stride2] * m.r20) + (pSrc[5 + stride2] * m.r21) + (pSrc[8 + stride2] * m.Pixel5) + (pSrc[11 + stride2] * m.r23) + (pSrc[14 + stride2] * m.r24);
                        sumr3 = (pSrc[2 + stride3] * m.r30) + (pSrc[5 + stride3] * m.r31) + (pSrc[8 + stride3] * m.r32) + (pSrc[11 + stride3] * m.r33) + (pSrc[14 + stride3] * m.r34);
                        sumr4 = (pSrc[2 + stride4] * m.r40) + (pSrc[5 + stride4] * m.r41) + (pSrc[8 + stride4] * m.r42) + (pSrc[11 + stride4] * m.r43) + (pSrc[14 + stride4] * m.r44);
                        nPixel = (((sumr0 + sumr1 + sumr2 + sumr3 + sumr4) / m.Factor) + m.Offset);
                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[8 + stride2] = (byte)nPixel;

                        // GREEN COLOR HERE   1 
                        sumr0 = (pSrc[1] * m.r00) + (pSrc[4] * m.r01) + (pSrc[7] * m.r02) + (pSrc[10] * m.r03) + (pSrc[13] * m.r04);
                        sumr1 = (pSrc[1 + stride] * m.r10) + (pSrc[4 + stride] * m.r11) + (pSrc[7 + stride] * m.r12) + (pSrc[10 + stride] * m.r13) + (pSrc[13 + stride] * m.r14);
                        sumr2 = (pSrc[1 + stride2] * m.r20) + (pSrc[4 + stride2] * m.r21) + (pSrc[7 + stride2] * m.Pixel5) + (pSrc[10 + stride2] * m.r23) + (pSrc[13 + stride2] * m.r24);
                        sumr3 = (pSrc[1 + stride3] * m.r30) + (pSrc[4 + stride3] * m.r31) + (pSrc[7 + stride3] * m.r32) + (pSrc[10 + stride3] * m.r33) + (pSrc[13 + stride3] * m.r34);
                        sumr4 = (pSrc[1 + stride4] * m.r40) + (pSrc[4 + stride4] * m.r41) + (pSrc[7 + stride4] * m.r42) + (pSrc[10 + stride4] * m.r43) + (pSrc[13 + stride4] * m.r44);
                        nPixel = (((sumr0 + sumr1 + sumr2 + sumr3 + sumr4) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[7 + stride2] = (byte)nPixel;

                        // BLUE COLOUR HERE 0 

                        sumr0 = (pSrc[0] * m.r00) + (pSrc[3] * m.r01) + (pSrc[6] * m.r02) + (pSrc[9] * m.r03) + (pSrc[12] * m.r04);
                        sumr1 = (pSrc[0 + stride] * m.r10) + (pSrc[3 + stride] * m.r11) + (pSrc[6 + stride] * m.r12) + (pSrc[9 + stride] * m.r13) + (pSrc[12 + stride] * m.r14);
                        sumr2 = (pSrc[0 + stride2] * m.r20) + (pSrc[3 + stride2] * m.r21) + (pSrc[6 + stride2] * m.Pixel5) + (pSrc[9 + stride2] * m.r23) + (pSrc[12 + stride2] * m.r24);
                        sumr3 = (pSrc[0 + stride3] * m.r30) + (pSrc[3 + stride3] * m.r31) + (pSrc[6 + stride3] * m.r32) + (pSrc[9 + stride3] * m.r33) + (pSrc[12 + stride3] * m.r34);
                        sumr4 = (pSrc[0 + stride4] * m.r40) + (pSrc[3 + stride4] * m.r41) + (pSrc[6 + stride4] * m.r42) + (pSrc[9 + stride4] * m.r43) + (pSrc[12 + stride4] * m.r44);
                        nPixel = (((sumr0 + sumr1 + sumr2 + sumr3 + sumr4) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[6 + stride2] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return true;
        }

        //#####################   SOBEL   7 BY   7   ##############################
        public static bool Conv7x7(Bitmap b, ConvMatrix7X7 m)
        {
            // Avoid divide by zero errors
            if (m.Factor == 0) return false;

            Bitmap bSrc = (Bitmap)b.Clone();

            // GDI+  -  returns format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            int stride3 = stride * 3;
            int stride4 = stride * 4;
            int stride5 = stride * 5;
            int stride6 = stride * 6;



            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 7;
                int nHeight = b.Height - 7;

                int nPixel = 0;
                int sumr0 = 0, sumr1 = 0, sumr2 = 0, sumr3 = 0, sumr4 = 0, sumr5 = 0, sumr6 = 0;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {

                        sumr0 = (pSrc[2] * m.r00) + (pSrc[5] * m.r01) + (pSrc[8] * m.r02) + (pSrc[11] * m.r03) + (pSrc[14] * m.r04) + (pSrc[17] * m.r05) + (pSrc[20] * m.r06);
                        sumr1 = (pSrc[2 + stride] * m.r10) + (pSrc[5 + stride] * m.r11) + (pSrc[8 + stride] * m.r12) + (pSrc[11 + stride] * m.r13) + (pSrc[14 + stride] * m.r14) + (pSrc[17 + stride] * m.r15) + (pSrc[20] * m.r16);
                        sumr2 = (pSrc[2 + stride2] * m.r20) + (pSrc[5 + stride2] * m.r21) + (pSrc[8 + stride2] * m.r22) + (pSrc[11 + stride2] * m.r23) + (pSrc[14 + stride2] * m.r24) + (pSrc[17 + stride2] * m.r25) + (pSrc[20 + stride2] * m.r26);
                        sumr3 = (pSrc[2 + stride3] * m.r30) + (pSrc[5 + stride3] * m.r31) + (pSrc[8 + stride3] * m.r32) + (pSrc[11 + stride3] * m.Pexil7) + (pSrc[14 + stride3] * m.r34) + (pSrc[17 + stride3] * m.r35) + (pSrc[20 + stride3] * m.r36);
                        sumr4 = (pSrc[2 + stride4] * m.r40) + (pSrc[5 + stride4] * m.r41) + (pSrc[8 + stride4] * m.r42) + (pSrc[11 + stride4] * m.r43) + (pSrc[14 + stride4] * m.r44) + (pSrc[17 + stride4] * m.r45) + (pSrc[20 + stride4] * m.r46);
                        sumr5 = (pSrc[2 + stride5] * m.r50) + (pSrc[5 + stride5] * m.r51) + (pSrc[8 + stride5] * m.r52) + (pSrc[11 + stride5] * m.r53) + (pSrc[14 + stride5] * m.r54) + (pSrc[17 + stride5] * m.r55) + (pSrc[20 + stride5] * m.r56);
                        sumr6 = (pSrc[2 + stride6] * m.r60) + (pSrc[5 + stride6] * m.r61) + (pSrc[8 + stride6] * m.r62) + (pSrc[11 + stride5] * m.r63) + (pSrc[14 + stride6] * m.r64) + (pSrc[17 + stride6] * m.r65) + (pSrc[20 + stride6] * m.r66);

                        nPixel = (((sumr0 + sumr1 + sumr2 + sumr3 + sumr4 + sumr5 + sumr6) / m.Factor) + m.Offset);
                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[8 + stride3] = (byte)nPixel;


                        sumr0 = (pSrc[1] * m.r00) + (pSrc[4] * m.r01) + (pSrc[7] * m.r02) + (pSrc[10] * m.r03) + (pSrc[13] * m.r04) + (pSrc[16] * m.r05) + (pSrc[18] * m.r06);
                        sumr1 = (pSrc[1 + stride] * m.r10) + (pSrc[4 + stride] * m.r11) + (pSrc[7 + stride] * m.r12) + (pSrc[10 + stride] * m.r13) + (pSrc[13 + stride] * m.r14) + (pSrc[16 + stride] * m.r15) + (pSrc[19 + stride] * m.r16);
                        sumr2 = (pSrc[1 + stride2] * m.r20) + (pSrc[4 + stride2] * m.r21) + (pSrc[7 + stride2] * m.r22) + (pSrc[10 + stride2] * m.r23) + (pSrc[13 + stride2] * m.r24) + (pSrc[16 + stride2] * m.r25) + (pSrc[19 + stride2] * m.r26);
                        sumr3 = (pSrc[1 + stride3] * m.r30) + (pSrc[4 + stride3] * m.r31) + (pSrc[7 + stride3] * m.r32) + (pSrc[10 + stride3] * m.Pexil7) + (pSrc[13 + stride3] * m.r34) + (pSrc[16 + stride3] * m.r35) + (pSrc[19 + stride3] * m.r36);
                        sumr4 = (pSrc[1 + stride4] * m.r40) + (pSrc[4 + stride4] * m.r41) + (pSrc[7 + stride4] * m.r42) + (pSrc[10 + stride4] * m.r43) + (pSrc[13 + stride4] * m.r44) + (pSrc[16 + stride4] * m.r45) + (pSrc[19 + stride4] * m.r46);
                        sumr5 = (pSrc[1 + stride5] * m.r50) + (pSrc[4 + stride5] * m.r51) + (pSrc[7 + stride5] * m.r52) + (pSrc[10 + stride5] * m.r53) + (pSrc[13 + stride4] * m.r54) + (pSrc[16 + stride5] * m.r55) + (pSrc[19 + stride5] * m.r56);
                        sumr6 = (pSrc[1 + stride6] * m.r60) + (pSrc[4 + stride6] * m.r61) + (pSrc[7 + stride6] * m.r62) + (pSrc[10 + stride6] * m.r63) + (pSrc[13 + stride6] * m.r64) + (pSrc[16 + stride6] * m.r65) + (pSrc[19 + stride6] * m.r66);
                        nPixel = (((sumr0 + sumr1 + sumr2 + sumr3 + sumr4 + sumr5 + sumr6) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[7 + stride3] = (byte)nPixel;



                        sumr0 = (pSrc[0] * m.r00) + (pSrc[3] * m.r01) + (pSrc[6] * m.r02) + (pSrc[9] * m.r03) + (pSrc[12] * m.r04) + (pSrc[15] * m.r05) + (pSrc[18] * m.r05);
                        sumr1 = (pSrc[0 + stride] * m.r10) + (pSrc[3 + stride] * m.r11) + (pSrc[6 + stride] * m.r12) + (pSrc[9 + stride] * m.r13) + (pSrc[12 + stride] * m.r14) + (pSrc[15 + stride] * m.r15) + (pSrc[18 + stride]);
                        sumr2 = (pSrc[0 + stride2] * m.r20) + (pSrc[3 + stride2] * m.r21) + (pSrc[6 + stride2] * m.r22) + (pSrc[9 + stride2] * m.r23) + (pSrc[12 + stride2] * m.r24) + (pSrc[15 + stride2] * m.r25) + (pSrc[15 + stride2] * m.r26);
                        sumr3 = (pSrc[0 + stride3] * m.r30) + (pSrc[3 + stride3] * m.r31) + (pSrc[6 + stride3] * m.r32) + (pSrc[9 + stride3] * m.Pexil7) + (pSrc[12 + stride3] * m.r34) + (pSrc[15 + stride3] * m.r35) + (pSrc[18 + stride3] * m.r36);
                        sumr4 = (pSrc[0 + stride4] * m.r40) + (pSrc[3 + stride4] * m.r41) + (pSrc[6 + stride4] * m.r42) + (pSrc[6 + stride4] * m.r43) + (pSrc[12 + stride4] * m.r44) + (pSrc[15 + stride4] * m.r45) + (pSrc[18 + stride4] * m.r46);
                        sumr5 = (pSrc[0 + stride5] * m.r50) + (pSrc[3 + stride5] * m.r51) + (pSrc[6 + stride5] * m.r52) + (pSrc[6 + stride5] * m.r53) + (pSrc[12 + stride5] * m.r54) + (pSrc[15 + stride5] * m.r55) + (pSrc[18 + stride5] * m.r56);
                        sumr5 = (pSrc[0 + stride6] * m.r60) + (pSrc[3 + stride6] * m.r61) + (pSrc[6 + stride6] * m.r62) + (pSrc[6 + stride6] * m.r63) + (pSrc[12 + stride6] * m.r64) + (pSrc[15 + stride6] * m.r65) + (pSrc[18 + stride6] * m.r66);
                        nPixel = (((sumr0 + sumr1 + sumr2 + sumr3 + sumr4 + sumr5 + sumr6) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[6 + stride3] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return true;
        }



        public static bool Smooth(Bitmap b, int nWeight /* default to 1 */)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.Factor = nWeight + 8;

            return BitmapFilter.Conv3x3(b, m);
        }

        public static bool GaussianBlur(Bitmap b, int nWeight /* default to 4*/)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            m.Factor = nWeight + 12;

            return BitmapFilter.Conv3x3(b, m);
        }


        public static bool EdgeDetectConvolution(Bitmap b, short SelectEdge, byte nThreshold)
        {
            ConvMatrix m = new ConvMatrix();
            ConvMatrix5X5 m5 = new ConvMatrix5X5();
            ConvMatrix7X7 m7 = new ConvMatrix7X7();

            int flageSob30 = 0, flageSob31 = 0;
            int flageSob70 = 0, flageSobl71 = 0;
            int flageSob50 = 0, flageSob51 = 0;
            // CANNY FLAGES 

            int flageCany30 = 0, flageCany31 = 0;
            int flageCany50 = 0, flageCany51 = 0;
            int flageCany70 = 0, flageCany71 = 0;
            Bitmap bTemp = (Bitmap)b.Clone();
            bool CANNYFLAGE = false;
            switch (SelectEdge)
            {
                case EDGE_DETECT_SOBEL3X3:
                    m.SetAll(0);
                    m.TopLeft = m.BottomLeft = 1;
                    m.TopRight = m.BottomRight = -1;    // GX =   here 3 by 3 
                    m.MidLeft = 2;
                    m.MidRight = -2;
                    m.Offset = 0;
                    flageSob30 = 1;
                    ;
                    break;


                case EDGE_DETECT_SOBEL5X5:

                    m5.SetAll(0);
                    m5.r00 = m5.r40 = 1;
                    m5.r01 = m5.r41 = 2;
                    m5.r03 = m5.r43 = -2;              // GX  = here  5 by 5 
                    m5.r04 = m5.r44 = -1;

                    m5.r10 = m5.r30 = 4;
                    m5.r11 = m5.r31 = 8;
                    m5.r13 = m5.r33 = -8;
                    m5.r14 = m5.r34 = -4;

                    m5.r20 = 6; m5.r21 = 12;
                    m5.r23 = -12; m5.r24 = -6;
                    m5.Offset = 0;
                    flageSob50 = 1;
                    break;

                case EDGE_DETECT_SOBEL7X7:
                    m7.r00 = m7.r60 = m7.r11 = m7.r22 = m7.r42 = m7.r51 = 3;   // GY = 7 by 7 
                    m7.r01 = m7.r12 = m7.r52 = m7.r61 = 2;
                    m7.r10 = m7.r21 = m7.r32 = m7.r41 = m7.r50 = 4;
                    m7.r20 = m7.r31 = m7.r40 = 5;
                    m7.r30 = 6;
                    m7.r06 = m7.r15 = m7.r24 = m7.r44 = m7.r55 = m7.r66 = -3;
                    m7.r05 = m7.r14 = m7.r54 = m7.r65 = -2;
                    m7.r16 = m7.r25 = m7.r34 = m7.r45 = m7.r56 = -4;
                    m7.r26 = m7.r35 = m7.r46 = -5;
                    m7.r36 = -6;
                    m7.Offset = 0;
                    flageSob70 = 1;

                    break;
                case EDGE_DETECT_CANNY3X3:
                    m.SetAll(0);
                    m.TopLeft = m.MidLeft = m.BottomLeft = -1;
                    m.TopRight = m.MidRight = m.BottomRight = 1;
                    m.Offset = 0;
                    flageCany30 = 1;
                    CANNYFLAGE = true;
                    break;

                case EDGE_DETECT_CANNY5X5:

                    m5.SetAll(0);
                    m5.r00 = m5.r40 = 1;
                    m5.r01 = m5.r41 = 2;
                    m5.r03 = m5.r43 = -2;              // GX  = here  5 by 5 
                    m5.r04 = m5.r44 = -1;

                    m5.r10 = m5.r30 = 4;
                    m5.r11 = m5.r31 = 8;
                    m5.r13 = m5.r33 = -8;
                    m5.r14 = m5.r34 = -4;

                    m5.r20 = 6; m5.r21 = 12;
                    m5.r23 = -12; m5.r24 = -6;
                    m5.Offset = 0;
                    flageCany50 = 1;
                    CANNYFLAGE = true;
                    break;

                case EDGE_DETECT_CANNY7X7:
                    flageCany70 = 1;
                    m7.r00 = m7.r60 = m7.r11 = m7.r22 = m7.r42 = m7.r51 = 3;   // GY = 7 by 7 
                    m7.r01 = m7.r12 = m7.r52 = m7.r61 = 2;
                    m7.r10 = m7.r21 = m7.r32 = m7.r41 = m7.r50 = 4;
                    m7.r20 = m7.r31 = m7.r40 = 5;
                    m7.r30 = 6;
                    m7.r06 = m7.r15 = m7.r24 = m7.r44 = m7.r55 = m7.r66 = -3;
                    m7.r05 = m7.r14 = m7.r54 = m7.r65 = -2;
                    m7.r16 = m7.r25 = m7.r34 = m7.r45 = m7.r56 = -4;
                    m7.r26 = m7.r35 = m7.r46 = -5;
                    m7.r36 = -6;
                    m7.Offset = 0;
                    flageCany70 = 1;
                    CANNYFLAGE = true;
                    break;




            }
            if (flageSob30 == 1) BitmapFilter.Conv3x3(b, m);
            if (flageSob50 == 1) BitmapFilter.Conv5x5(b, m5);
            if (flageSob70 == 1) BitmapFilter.Conv7x7(b, m7);

            if (flageCany30 == 1) BitmapFilter.Conv3x3(b, m);
            if (flageCany50 == 1) BitmapFilter.Conv5x5(b, m5);
            if (flageCany70 == 1) BitmapFilter.Conv7x7(b, m7);  // CANNY 7 By 7 

            switch (SelectEdge)
            {
                case EDGE_DETECT_SOBEL3X3:
                    m.SetAll(0);
                    m.TopLeft = m.TopRight = 1;
                    m.BottomLeft = m.BottomRight = -1;
                    m.TopMid = 2;
                    m.BottomMid = -2;
                    m.Offset = 0;
                    flageSob31 = 1;
                    break;

                case EDGE_DETECT_SOBEL5X5:

                    m5.SetAll(0);
                    m5.r00 = m5.r04 = 1;
                    m5.r01 = m5.r03 = 4;
                    m5.r02 = 6;



                    m5.r10 = m5.r14 = 2;      // GY = here 
                    m5.r11 = m5.r13 = 8;
                    m5.r12 = 12;


                    m5.r30 = m5.r34 = -2;
                    m5.r31 = m5.r33 = -8;
                    m5.r32 = -12;

                    m5.r40 = m5.r44 = -1;
                    m5.r41 = m5.r43 = -4;
                    m5.r42 = -6;
                    m5.Offset = 0;
                    flageSob51 = 1;
                    break;

                case EDGE_DETECT_SOBEL7X7:
                    m7.r00 = m7.r06 = m7.r11 = m7.r15 = m7.r22 = m7.r24 = 3;
                    m7.r01 = m7.r05 = m7.r12 = m7.r14 = m7.r23 = 4;
                    m7.r02 = m7.r04 = m7.r13 = 5;
                    m7.r03 = 6;
                    m7.r10 = m7.r16 = m7.r21 = m7.r25 = 2;
                    m7.r20 = m7.r26 = 1;
                    m7.r60 = m7.r51 = m7.r42 = m7.r66 = m7.r55 = m7.r44 = -3;
                    m7.r61 = m7.r65 = m7.r52 = m7.r54 = m7.r43 = -4;
                    m7.r62 = m7.r64 = m7.r53 = -5;
                    m7.r63 = -6;
                    m7.r50 = m7.r56 = m7.r41 = m7.r45 = -2;
                    m7.r40 = m7.r46 = -1;
                    m7.Offset = 0;
                    flageSobl71 = 1;
                    break;
                case EDGE_DETECT_CANNY3X3:
                    m.SetAll(0);
                    m.BottomLeft = m.BottomMid = m.BottomRight = -1;
                    m.TopLeft = m.TopMid = m.TopRight = 1;
                    m.Offset = 0;
                    flageCany31 = 1;
                    CANNYFLAGE = true;
                    break;


                case EDGE_DETECT_CANNY5X5:

                    m5.SetAll(0);
                    m5.r00 = m5.r04 = -1;
                    m5.r01 = m5.r03 = -4;
                    m5.r02 = -6;
                    m5.r10 = m5.r14 = -2;      // GY = here 
                    m5.r11 = m5.r13 = -8;
                    m5.r12 = -12;
                    m5.r30 = m5.r34 = 2;
                    m5.r31 = m5.r33 = 8;
                    m5.r32 = 12;
                    m5.r40 = m5.r44 = 1;
                    m5.r41 = m5.r43 = 4;
                    m5.r42 = 6;
                    m5.Offset = 0;
                    flageCany51 = 1;
                    CANNYFLAGE = true;
                    break;
                case EDGE_DETECT_CANNY7X7:
                    flageCany71 = 1;
                    m7.r00 = m7.r06 = m7.r11 = m7.r15 = m7.r22 = m7.r24 = 3;
                    m7.r01 = m7.r05 = m7.r12 = m7.r14 = m7.r23 = 4;
                    m7.r02 = m7.r04 = m7.r13 = 5;
                    m7.r03 = 6;
                    m7.r10 = m7.r16 = m7.r21 = m7.r25 = 2;
                    m7.r20 = m7.r26 = 1;
                    m7.r60 = m7.r51 = m7.r42 = m7.r66 = m7.r55 = m7.r44 = -3;
                    m7.r61 = m7.r65 = m7.r52 = m7.r54 = m7.r43 = -4;
                    m7.r62 = m7.r64 = m7.r53 = -5;
                    m7.r63 = -6;
                    m7.r50 = m7.r56 = m7.r41 = m7.r45 = -2;
                    m7.r40 = m7.r46 = -1;
                    m7.Offset = 0;
                    flageCany71 = 1;
                    CANNYFLAGE = true;
                    break;

            }
            if (flageSob31 == 1) BitmapFilter.Conv3x3(bTemp, m);   // SOBEL 3 BY 3  
            if (flageSob51 == 1) BitmapFilter.Conv5x5(bTemp, m5);  // SOBEL 5 BY 5 
            if (flageSobl71 == 1) BitmapFilter.Conv7x7(bTemp, m7);  // SOBEL 7 BY 7 

            if (flageCany31 == 1) BitmapFilter.Conv3x3(bTemp, m);   // CANNY 3 BY 3 
            if (flageCany51 == 1) BitmapFilter.Conv5x5(bTemp, m5);     // CANNY 5 BY 5 
            if (flageCany71 == 1) BitmapFilter.Conv7x7(bTemp, m7);  // CANNY 7 By 7 

            // GDI+ --- returns format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData2 = bTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //###################  (MODIFIED 23-4-2010  for CANNY ) #####################
            Bitmap btemp3 = (Bitmap)b.Clone();
            BitmapData bmData3 = btemp3.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.IntPtr Scan03 = bmData3.Scan0;
            // double  pi= Math.PI; 

            //############################################################################
            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;
            int offset = stride - b.Width;
            int nWidth = b.Width * 3;
            int ptr = b.Width;
            int nPixel = 0;
            int nOffset = stride - b.Width * 3;
            double div;
            byte leftPixel = 0, rightPixel = 0;
            double orientation = 0, toAngle = 180 / Math.PI;
            byte  highThreshold=100 ,lowThrshold=10 ;
            byte[] orients = new byte[b.Width * b.Height];
            int count=0; 
            unsafe
            {
                byte* p = (byte*)(void*)Scan0.ToPointer()+stride;
                byte* p2 = (byte*)(void*)Scan02.ToPointer() +stride;

                for (int y = 0; y < b.Height-1; y++)
                {
                                   
                    
                    for (int x = 0; x <b.Width-1; x++,p++,p2++,count++)
                    {
                        if (*p == 0) if (*p2 == 0) orientation = 90;

                           else
                            {
                               div = *p/ *p2;
                               if (div < 0) orientation = 180 - Math.Atan(-div) * toAngle;
                                  else orientation = Math.Atan(div) * toAngle; 

                                if (orientation < 22.5) orientation = 0;
                                else if (orientation < 67.5) orientation = 45;
                                else if (orientation < 112.5) orientation = 90;
                                else if (orientation < 157.5) orientation = 135;
                                else orientation = 0;
                            }
                        orients[count] = (byte)orientation;
                    nPixel =(int) Math.Min (Math.Abs(*p+*p2),255) ;
                    *p = (byte)nPixel;
                                    
                    
                    }
                    p += nOffset+1;
                    p2 += nOffset + 1; 
                }

                 
                 p = (byte*)(void*)Scan0.ToPointer()+ stride;
                 p2 = (byte*)(void*)Scan02.ToPointer()+ stride;
                //************ CANNY Eo GX +GY  bmData3  POINTER   *********
               
              
                p2 = (byte*)Scan02.ToPointer() + stride;
                p = (byte*)Scan0.ToPointer() + stride;

                count  = 0;

                if (CANNYFLAGE)
                {
                   
                        for (int y = 1; y < b.Height - 1; y++)
                        {

                            p++; 
                            for (int x = 1; x < b.Width - 1; x++, p++, count++)
                            {

                                switch (orients[count])
                                {
                                    case 0: leftPixel = p[-1];
                                        rightPixel = p[1];
                                        break;
                                    case 45: leftPixel = p[stride - 1];
                                        rightPixel = p[-stride + 1];
                                        break;
                                    case 90: leftPixel = p[stride];
                                        rightPixel = p[-stride];
                                        break;
                                    case 153: leftPixel = p[stride + 1];
                                        rightPixel = p[-stride - 1];
                                        break;
                                }
                                if ((*p < leftPixel) || (*p < rightPixel)) *p = 0;
                                

                            }
                            p += (offset + 1);

                            ++count;

                        }


                        p = (byte*)Scan0.ToPointer() + stride;



                        for (int y = 1; y < b.Height - 2; y++)
                        {
                            p++;

                            for (int x = 1; x < b.Width - 2; x++, p++)
                            {
                                if (p[0] < highThreshold)
                                {
                                    if (p[0] < lowThrshold)
                                        p[0] = 0;

                                    else
                                    {
                                        if ((p[-1] < highThreshold) &&
                                            (p[1] < highThreshold) &&
                                            (p[-stride - 1] < highThreshold) &&
                                            (p[-stride] < highThreshold) &&
                                            (p[-stride + 1] < highThreshold) &&
                                            (p[stride - 1] < highThreshold) &&
                                                (p[stride] < highThreshold) &&
                                                (p[stride + 1] < highThreshold))
                                        {
                                            p[0] = 0;
                                        }
                                    }
                                }


                            }
                            p += (offset + 1);


                        }


                    }
                



                  }




               
                b.UnlockBits(bmData);
                bTemp.UnlockBits(bmData2);

                return true;






            }
        }
    }

