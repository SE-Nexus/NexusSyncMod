using System;

namespace NexusSyncMod.Render.MathTools
{
    // Code originally from: https://pastebin.com/FBta2RVT by jTurp

    public struct Complex
    {
        private const double LOG_10_INV = 0.43429448190325;
        private const double eps = 1e-6;
        public double Re;
        public double Im;

        public Complex(double re, double im)
        {
            Re = re;
            Im = im;
        }

        #region Attributes
        public static readonly Complex Zero = new Complex(0.0, 0.0);
        public static readonly Complex Re_One = new Complex(1.0, 0.0);
        public static readonly Complex Im_One = new Complex(0.0, 1.0);
        #endregion

        #region Operators
        public static Complex operator +(Complex a, Complex b)
        {
            Complex result = new Complex(
                a.Re + b.Re,
                a.Im + b.Im);
            return result;
        }

        public static Complex operator -(Complex a, Complex b)
        {
            Complex result = new Complex(
                a.Re - b.Re,
                a.Im - b.Im);
            return result;
        }

        public static Complex operator *(Complex a, Complex b)
        {
            Complex result = new Complex(
                a.Re * b.Re - a.Im * b.Im,
                a.Im * b.Re + b.Im * a.Re);
            return result;
        }

        public static Complex operator /(Complex a, Complex b)
        {
            Complex result = new Complex(
                (a.Re * b.Re + a.Im * b.Im) / (b.Re * b.Re + b.Im * b.Im),
                (a.Im * b.Re - a.Re * b.Im) / (b.Re * b.Re + b.Im * b.Im));
            return result;
        }

        public static bool operator ==(Complex a, double b)
        {
            return a.Re == b && a.Im == b;
        }

        public static bool operator !=(Complex a, double b)
        {
            return !(a.Re == b && a.Im == b);
        }

        public static bool operator >(Complex a, Complex b)
        {
            return a.Re > b.Re && a.Im > b.Im;
        }

        public static bool operator <(Complex a, Complex b)
        {
            return a.Re < b.Re && a.Im < b.Im;
        }

        public static bool operator >=(Complex a, Complex b)
        {
            return a.Re >= b.Re && a.Im >= b.Im;
        }

        public static bool operator <=(Complex a, Complex b)
        {
            return a.Re <= b.Re && a.Im <= b.Im;
        }

        public static bool operator >(Complex a, double b)
        {
            return a.Re > b && a.Im > b;
        }

        public static bool operator <(Complex a, double b)
        {
            return a.Re < b && a.Im < b;
        }

        public static bool operator >=(Complex a, double b)
        {
            return a.Re >= b && a.Im >= b;
        }

        public static bool operator <=(Complex a, double b)
        {
            return a.Re <= b && a.Im <= b;
        }
        public static Complex operator *(Complex a, double b)
        {
            Complex B = new Complex(b, 0);
            return a * B;
        }

        public static Complex operator /(Complex a, double b)
        {
            Complex B = new Complex(b, 0);
            return a / B;
        }

        public static Complex operator +(Complex a, double b)
        {
            Complex B = new Complex(b, 0);
            return a + B;
        }

        public static Complex operator -(Complex a, double b)
        {
            Complex B = new Complex(b, 0);
            return a - B;
        }

        public static Complex operator *(double a, Complex b)
        {
            Complex A = new Complex(a, 0);
            return A * b;
        }

        public static Complex operator /(double a, Complex b)
        {
            Complex A = new Complex(a, 0);
            return A / b;
        }

        public static Complex operator +(double a, Complex b)
        {
            Complex A = new Complex(a, 0);
            return A + b;
        }

        public static Complex operator -(double a, Complex b)
        {
            Complex A = new Complex(a, 0);
            return A - b;
        }

        public static Complex Sqrt(Complex a)
        {
            return Pow(a, 0.5f);
        }

        public static Complex Sqrt(double a)
        {
            return Pow(new Complex(a, 0), 0.5f);
        }

        public static Complex Pow(Complex a, double b)
        {
            // De Moivre's Theorem:  (r*(cos(x) + i*sin(x))^n = r^n*(cos(x*n) + i*sin(x*n))

            // First convert the complex number to polar form...
            double r = Math.Sqrt(a.Re * a.Re + a.Im * a.Im);
            double x = Math.Atan2(a.Im, a.Re);

            // And then rebuild the components according to De Moivre... 
            double cos = Math.Pow(r, b) * Math.Cos(x * b);
            double sin = Math.Pow(r, b) * Math.Sin(x * b);
            return new Complex(cos, sin);
        }

        public static Complex Pow(double a, double b)
        {
            return Pow(new Complex(a, 0), b);
        }

        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Re, -a.Im);
        }
        #endregion

        #region Trig Functions
        public static Complex Sin(Complex a)
        {
            double a_Re = a.Re;
            double a_Im = a.Im;
            return new Complex(Math.Sin(a_Re) * Math.Cosh(a_Im), Math.Cos(a_Re) * Math.Sinh(a_Im));
        }

        public static Complex Sinh(Complex a) /* Hyperbolic sin */
        {
            double a_Re = a.Re;
            double a_Im = a.Im;
            return new Complex(Math.Sinh(a_Re) * Math.Cos(a_Im), Math.Cosh(a_Re) * Math.Sin(a_Im));

        }
        public static Complex Asin(Complex a) /* Arcsin */
        {
            return (-Im_One) * Log(Im_One * a + Sqrt(Re_One - a * a));
        }

        public static Complex Cos(Complex a)
        {
            double a_Re = a.Re;
            double a_Im = a.Im;
            return new Complex(Math.Cos(a_Re) * Math.Cosh(a_Im), -(Math.Sin(a_Re) * Math.Sinh(a_Im)));
        }

        public static Complex Cosh(Complex a) /* Hyperbolic cos */
        {
            double a_Re = a.Re;
            double a_Im = a.Im;
            return new Complex(Math.Cosh(a_Re) * Math.Cos(a_Im), Math.Sinh(a_Re) * Math.Sin(a_Im));
        }
        public static Complex Acos(Complex a) /* Arccos */
        {
            return (-Im_One) * Log(a + Im_One * Sqrt(Re_One - (a * a)));
        }
        public static Complex Tan(Complex a)
        {
            return Sin(a) / Cos(a);
        }

        public static Complex Tanh(Complex a) /* Hyperbolic tan */
        {
            return Sinh(a) / Cosh(a);
        }
        public static Complex Atan(Complex a) /* Arctan */
        {
            Complex Two = new Complex(2.0, 0.0);
            return (Im_One / Two) * (Log(Re_One - Im_One * a) - Log(Re_One + Im_One * a));
        }
        #endregion

        #region Other Functions
        public static Complex Log(Complex a) /* Log of the complex number value to the base of 'e' */
        {
            return new Complex(Math.Log(Abs(a)), Math.Atan2(a.Im, a.Re));
        }
        public static Complex Log(Complex a, Double b) /* Log of the complex number to a the base of a double */
        {
            return Log(a) / Math.Log(b);
        }
        public static Complex Log10(Complex a) /* Log to the base of 10 of the complex number */
        {
            Complex temp_log = Log(a);
            return Scale(temp_log, (Double)LOG_10_INV);
        }

        public bool Equals(Complex value)
        {
            return Re == value.Re && Im == value.Im;
        }

        public static double Abs(Complex a)
        {
            return Math.Sqrt(a.Im * a.Im + a.Re * a.Re);
        }

        public static bool IsNaN(Complex a)
        {
            return double.IsNaN(a.Re) || double.IsNaN(a.Im);
        }

        public static bool IsReal(Complex a)
        {
            if (Math.Abs(a.Im) < eps)
            { return true; }
            else
            { return false; }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return $"({Re}, {Im})";
        }

        public override int GetHashCode()
        {
            int n1 = 99999997;
            int hash_real = this.Re.GetHashCode() % n1;
            int hash_imaginary = this.Im.GetHashCode();
            int final_hashcode = hash_real ^ hash_imaginary;
            return final_hashcode;
        }
        #endregion

        #region Internal Functions
        private static Complex Scale(Complex a, double b)
        {
            double result_re = b * a.Re;
            double result_im = b * a.Im;
            return new Complex(result_re, result_im);
        }
        #endregion

        public static bool IsZero(Complex d)
        {
            return d > -eps && d < eps;
        }
    }
}
