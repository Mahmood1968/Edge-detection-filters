using System;
using System.Drawing;
using System.Drawing.Imaging;

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


	public class BitmapFilter
	{
		public const short EDGE_DETECT_KIRSH		= 1;
		public const short EDGE_DETECT_PREWITT		= 2;
		public const short EDGE_DETECT_SOBEL		= 3;

		public static bool Invert(Bitmap b)
		{
			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;
	
				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < nWidth; ++x )
					{
						p[0] = (byte)(255-p[0]);
						++p;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return true;
		}

		public static bool GrayScale(Bitmap b)
		{
			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				byte red, green, blue;
	
				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < b.Width; ++x )
					{
						blue = p[0];
						green = p[1];
						red = p[2];

						p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return true;
		}

		public static bool Brightness(Bitmap b, int nBrightness)
		{
			if (nBrightness < -255 || nBrightness > 255)
				return false;

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			int nVal = 0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < nWidth; ++x )
					{
						nVal = (int) (p[0] + nBrightness);
		
						if (nVal < 0) nVal = 0;
						if (nVal > 255) nVal = 255;

						p[0] = (byte)nVal;

						++p;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return true;
		}

		public static bool Contrast(Bitmap b, sbyte nContrast)
		{
			if (nContrast < -100) return false;
			if (nContrast >  100) return false;

			double pixel = 0, contrast = (100.0+nContrast)/100.0;

			contrast *= contrast;

			int red, green, blue;
			
			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < b.Width; ++x )
					{
						blue = p[0];
						green = p[1];
						red = p[2];
				
						pixel = red/255.0;
						pixel -= 0.5;
						pixel *= contrast;
						pixel += 0.5;
						pixel *= 255;
						if (pixel < 0) pixel = 0;
						if (pixel > 255) pixel = 255;
						p[2] = (byte) pixel;

						pixel = green/255.0;
						pixel -= 0.5;
						pixel *= contrast;
						pixel += 0.5;
						pixel *= 255;
						if (pixel < 0) pixel = 0;
						if (pixel > 255) pixel = 255;
						p[1] = (byte) pixel;

						pixel = blue/255.0;
						pixel -= 0.5;
						pixel *= contrast;
						pixel += 0.5;
						pixel *= 255;
						if (pixel < 0) pixel = 0;
						if (pixel > 255) pixel = 255;
						p[0] = (byte) pixel;					

						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return true;
		}
	
		public static bool Gamma(Bitmap b, double red, double green, double blue)
		{
			if (red < .2 || red > 5) return false;
			if (green < .2 || green > 5) return false;
			if (blue < .2 || blue > 5) return false;

			byte [] redGamma = new byte [256];
			byte [] greenGamma = new byte [256];
			byte [] blueGamma = new byte [256];

			for (int i = 0; i< 256; ++i)
			{
				redGamma[i] = (byte)Math.Min(255, (int)(( 255.0 * Math.Pow(i/255.0, 1.0/red)) + 0.5));
				greenGamma[i] = (byte)Math.Min(255, (int)(( 255.0 * Math.Pow(i/255.0, 1.0/green)) + 0.5));
				blueGamma[i] = (byte)Math.Min(255, (int)(( 255.0 * Math.Pow(i/255.0, 1.0/blue)) + 0.5));
			}

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;

				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < b.Width; ++x )
					{
						p[2] = redGamma[ p[2] ];
						p[1] = greenGamma[ p[1] ];
						p[0] = blueGamma[ p[0] ];

						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return true;
		}

		public static bool Color(Bitmap b, int red, int green, int blue)
		{
			if (red < -255 || red > 255) return false;
			if (green < -255 || green > 255) return false;
			if (blue < -255 || blue > 255) return false;

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;

				int nOffset = stride - b.Width*3;
				int nPixel;

				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < b.Width; ++x )
					{
						nPixel = p[2] + red;
						nPixel = Math.Max(nPixel, 0);
						p[2] = (byte)Math.Min(255, nPixel);

						nPixel = p[1] + green;
						nPixel = Math.Max(nPixel, 0);
						p[1] = (byte)Math.Min(255, nPixel);

						nPixel = p[0] + blue;
						nPixel = Math.Max(nPixel, 0);
						p[0] = (byte)Math.Min(255, nPixel);

						p += 3;
					}
					p += nOffset;
				}
			}

			b.UnlockBits(bmData);

			return true;
		}

		public static bool Conv3x3(Bitmap b, ConvMatrix m)
		{
			// Avoid divide by zero errors
			if (0 == m.Factor) return false;

			Bitmap bSrc = (Bitmap)b.Clone(); 

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			int stride2 = stride * 2;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcScan0 = bmSrc.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrc = (byte *)(void *)SrcScan0;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width - 2;
				int nHeight = b.Height - 2;

				int nPixel;

				for(int y=0;y < nHeight;++y)
				{
					for(int x=0; x < nWidth; ++x )
					{
						nPixel = ( ( ( (pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
							(pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
							(pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset); 

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[5 + stride]= (byte)nPixel;

						nPixel = ( ( ( (pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
							(pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
							(pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset); 

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;
							
						p[4 + stride] = (byte)nPixel;

						nPixel = ( ( ( (pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
							(pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
							(pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset); 

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
		public static bool Smooth(Bitmap b, int nWeight /* default to 1 */)
		{
			ConvMatrix m = new ConvMatrix();
			m.SetAll(1);
			m.Pixel = nWeight;
			m.Factor = nWeight + 8;

			return  BitmapFilter.Conv3x3(b, m);
		}

		public static bool GaussianBlur(Bitmap b, int nWeight /* default to 4*/)
		{
			ConvMatrix m = new ConvMatrix();
			m.SetAll(1);
			m.Pixel = nWeight;
			m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
			m.Factor = nWeight + 12;

			return  BitmapFilter.Conv3x3(b, m);
		}
		public static bool MeanRemoval(Bitmap b, int nWeight /* default to 9*/ )
		{
			ConvMatrix m = new ConvMatrix();
			m.SetAll(-1);
			m.Pixel = nWeight;
			m.Factor = nWeight - 8;

			return BitmapFilter.Conv3x3(b, m);
		}
		public static bool Sharpen(Bitmap b, int nWeight /* default to 11*/ )
		{
			ConvMatrix m = new ConvMatrix();
			m.SetAll(0);
			m.Pixel = nWeight;
			m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
			m.Factor = nWeight - 8;

			return  BitmapFilter.Conv3x3(b, m);
		}
		public static bool EmbossLaplacian(Bitmap b)
		{
			ConvMatrix m = new ConvMatrix();
			m.SetAll(-1);
			m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 0;
			m.Pixel = 4;
			m.Offset = 127;

			return  BitmapFilter.Conv3x3(b, m);
		}	
		public static bool EdgeDetectQuick(Bitmap b)
		{
			ConvMatrix m = new ConvMatrix();
			m.TopLeft = m.TopMid = m.TopRight = -1;
			m.MidLeft = m.Pixel = m.MidRight = 0;
			m.BottomLeft = m.BottomMid = m.BottomRight = 1;
		
			m.Offset = 127;

			return  BitmapFilter.Conv3x3(b, m);
		}

		public static bool EdgeDetectConvolution(Bitmap b, short nType, byte nThreshold)
		{
			ConvMatrix m = new ConvMatrix();

			// I need to make a copy of this bitmap BEFORE I alter it 80)
			Bitmap bTemp = (Bitmap)b.Clone();

			switch (nType)
			{
				case EDGE_DETECT_SOBEL:
					m.SetAll(0);
					m.TopLeft = m.BottomLeft = 1;
					m.TopRight = m.BottomRight = -1;
					m.MidLeft = 2;
					m.MidRight = -2;
					m.Offset = 0;
					break;
				case EDGE_DETECT_PREWITT:
					m.SetAll(0);
					m.TopLeft = m.MidLeft = m.BottomLeft = -1;
					m.TopRight = m.MidRight = m.BottomRight = 1;
					m.Offset = 0;
					break;
				case EDGE_DETECT_KIRSH:
					m.SetAll(-3);
					m.Pixel = 0;
					m.TopLeft = m.MidLeft = m.BottomLeft = 5;
					m.Offset = 0;
					break;
			}

			BitmapFilter.Conv3x3(b, m);

			switch (nType)
			{
				case EDGE_DETECT_SOBEL:
					m.SetAll(0);
					m.TopLeft = m.TopRight = 1;
					m.BottomLeft = m.BottomRight = -1;
					m.TopMid = 2;
					m.BottomMid = -2;
					m.Offset = 0;
					break;
				case EDGE_DETECT_PREWITT:
					m.SetAll(0);
					m.BottomLeft = m.BottomMid = m.BottomRight = -1;
					m.TopLeft = m.TopMid = m.TopRight = 1;
					m.Offset = 0;
					break;
				case EDGE_DETECT_KIRSH:
					m.SetAll(-3);
					m.Pixel = 0;
					m.BottomLeft = m.BottomMid = m.BottomRight = 5;
					m.Offset = 0;
					break;
			}

			BitmapFilter.Conv3x3(bTemp, m);

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = bTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * p2 = (byte *)(void *)Scan02;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				int nPixel = 0;
	
				for(int y=0;y<b.Height;++y)
				{
					for(int x=0; x < nWidth; ++x )
					{
						nPixel = (int) Math.Sqrt((p[0]*p[0]) + (p2[0] * p2[0]));
						if (nPixel<nThreshold)nPixel = nThreshold;
						if (nPixel>255) nPixel = 255;
						p[0] = (byte) nPixel;
						++p;
						++p2;
					}
					p += nOffset;
					p2 += nOffset;
				}
			}

			b.UnlockBits(bmData);
			bTemp.UnlockBits(bmData2);

			return true;
		}
	
		public static bool EdgeDetectHorizontal(Bitmap b)
		{
			Bitmap bmTemp = (Bitmap)b.Clone();

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = bmTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * p2 = (byte *)(void *)Scan02;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				int nPixel = 0;
	
				p += stride;
				p2 += stride;

				for(int y=1;y<b.Height-1;++y)
				{
					p += 9;
					p2 += 9;

					for(int x=9; x < nWidth-9; ++x )
					{
						nPixel = ((p2 + stride - 9)[0] +
							(p2 + stride - 6)[0] +
							(p2 + stride - 3)[0] +
							(p2 + stride)[0] +
							(p2 + stride + 3)[0] +
							(p2 + stride + 6)[0] +
							(p2 + stride + 9)[0] -
							(p2 - stride - 9)[0] -
							(p2 - stride - 6)[0] -
							(p2 - stride - 3)[0] -
							(p2 - stride)[0] -
							(p2 - stride + 3)[0] -
							(p2 - stride + 6)[0] -
							(p2 - stride + 9)[0]);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						(p+stride)[0] = (byte) nPixel;
					
						++ p;
						++ p2;
					}

					p += 9 + nOffset;
					p2 += 9 + nOffset;
				}
			}

			b.UnlockBits(bmData);
			bmTemp.UnlockBits(bmData2);

			return true;
		}

		public static bool EdgeDetectVertical(Bitmap b)
		{
			Bitmap bmTemp = (Bitmap)b.Clone();

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = bmTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * p2 = (byte *)(void *)Scan02;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				int nPixel = 0;

				int nStride2 = stride *2;
				int nStride3 = stride * 3;
	
				p += nStride3;
				p2 += nStride3;

				for(int y=3;y<b.Height-3;++y)
				{
					p += 3;
					p2 += 3;

					for(int x=3; x < nWidth-3; ++x )
					{
						nPixel = ((p2 + nStride3 + 3)[0] +
							(p2 + nStride2 + 3)[0] +
							(p2 + stride + 3)[0] +
							(p2 + 3)[0] +
							(p2 - stride + 3)[0] +
							(p2 - nStride2 + 3)[0] +
							(p2 - nStride3 + 3)[0] -
							(p2 + nStride3 - 3)[0] -
							(p2 + nStride2 - 3)[0] -
							(p2 + stride - 3)[0] -
							(p2 - 3)[0] -
							(p2 - stride - 3)[0] -
							(p2 - nStride2 - 3)[0] -
							(p2 - nStride3 - 3)[0]);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[0] = (byte) nPixel;
					
						++ p;
						++ p2;
					}

					p += 3 + nOffset;
					p2 += 3 + nOffset;
				}
			}

			b.UnlockBits(bmData);
			bmTemp.UnlockBits(bmData2);

			return true;
		}

		public static bool EdgeDetectHomogenity(Bitmap b, byte nThreshold)
		{
			// This one works by working out the greatest difference between a pixel and it's eight neighbours.
			// The threshold allows softer edges to be forced down to black, use 0 to negate it's effect.
			Bitmap b2 = (Bitmap) b.Clone();

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * p2 = (byte *)(void *)Scan02;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				int nPixel = 0, nPixelMax = 0;

				p += stride;
				p2 += stride;

				for(int y=1;y<b.Height-1;++y)
				{
					p += 3;
					p2 += 3;

					for(int x=3; x < nWidth-3; ++x )
					{
						nPixelMax = Math.Abs(p2[0] - (p2+stride-3)[0]);
						nPixel = Math.Abs(p2[0] - (p2 + stride)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs(p2[0] - (p2 + stride + 3)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs(p2[0] - (p2 - stride)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs(p2[0] - (p2 + stride)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs(p2[0] - (p2 - stride - 3)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs(p2[0] - (p2 - stride)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs(p2[0] - (p2 - stride + 3)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						if (nPixelMax < nThreshold) nPixelMax = 0;

						p[0] = (byte) nPixelMax;

						++ p;
						++ p2;
					}

					p += 3 + nOffset;
					p2 += 3 + nOffset;
				}
			}

			b.UnlockBits(bmData);
			b2.UnlockBits(bmData2);

			return true;
            
		}
		public static bool EdgeDetectDifference(Bitmap b, byte nThreshold)
		{
			// This one works by working out the greatest difference between a pixel and it's eight neighbours.
			// The threshold allows softer edges to be forced down to black, use 0 to negate it's effect.
			Bitmap b2 = (Bitmap) b.Clone();

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * p2 = (byte *)(void *)Scan02;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				int nPixel = 0, nPixelMax = 0;

				p += stride;
				p2 += stride;

				for(int y=1;y<b.Height-1;++y)
				{
					p += 3;
					p2 += 3;

					for(int x=3; x < nWidth-3; ++x )
					{
						nPixelMax = Math.Abs((p2 - stride + 3)[0] - (p2+stride-3)[0]);
						nPixel = Math.Abs((p2 + stride + 3)[0] - (p2 - stride - 3)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs((p2 - stride)[0] - (p2 + stride)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs((p2+3)[0] - (p2 - 3)[0]);
						if (nPixel>nPixelMax) nPixelMax = nPixel;

						if (nPixelMax < nThreshold) nPixelMax = 0;

						p[0] = (byte) nPixelMax;

						++ p;
						++ p2;
					}

					p += 3 + nOffset;
					p2 += 3 + nOffset;
				}
			}

			b.UnlockBits(bmData);
			b2.UnlockBits(bmData2);

			return true;
            
		}

		public static bool EdgeEnhance(Bitmap b, byte nThreshold)
		{
			// This one works by working out the greatest difference between a nPixel and it's eight neighbours.
			// The threshold allows softer edges to be forced down to black, use 0 to negate it's effect.
			Bitmap b2 = (Bitmap) b.Clone();

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * p2 = (byte *)(void *)Scan02;

				int nOffset = stride - b.Width*3;
				int nWidth = b.Width * 3;

				int nPixel = 0, nPixelMax = 0;

				p += stride;
				p2 += stride;

				for (int y = 1; y < b.Height-1; ++y)
				{
					p += 3;
					p2 += 3;

					for (int x = 3; x < nWidth-3; ++x)
					{
						nPixelMax = Math.Abs((p2 - stride + 3)[0] - (p2 + stride - 3)[0]);

						nPixel = Math.Abs((p2 + stride + 3)[0] - (p2 - stride - 3)[0]);

						if (nPixel > nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs((p2 - stride)[0] - (p2 + stride)[0]);

						if (nPixel > nPixelMax) nPixelMax = nPixel;

						nPixel = Math.Abs((p2 + 3)[0] - (p2 - 3)[0]);

						if (nPixel > nPixelMax) nPixelMax = nPixel;

						if (nPixelMax > nThreshold && nPixelMax > p[0])
							p[0] = (byte) Math.Max(p[0], nPixelMax);

						++ p;
						++ p2;			
					}

					p += nOffset + 3;
					p2 += nOffset + 3;
				}
			}	

			b.UnlockBits(bmData);
			b2.UnlockBits(bmData2);

			return true;
		}
	}
}
