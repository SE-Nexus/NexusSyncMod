using System;
using System.Collections.Generic;

namespace NexusSyncMod.Render.MathTools
{
    // Code originally from: https://pastebin.com/FBta2RVT by jTurp

    public class QuarticSolver
    {
        private const double _eps = 1e-6;

        private readonly List<double> _realRoots = new List<double>();
        private List<Complex> _complexRoots = new List<Complex>();

        public QuarticSolver()
        {
        }

        // MathForum.org solution for Quartic Polynomial
        public List<double> SolveQuartic(double a4, double a3, double a2, double a1, double a0)
        {
            // Starting equation: [a4]x^4 + [a3]x^3 + [a2]x^2 + [a1]x + [a0] = 0
            _realRoots.Clear();
            _complexRoots.Clear();

            if (Math.Abs(a4) < _eps && Math.Abs(a3) < _eps)
            {
                // Quadratic case - true if max speed has been reached
                _complexRoots = SolveQuadratic(a2, a1, a0);

                foreach (Complex t in _complexRoots)
                {
                    if (!Complex.IsNaN(t) && Complex.IsReal(t))
                    { _realRoots.Add(t.Re); }
                }
            }
            else if (Math.Abs(a4) < _eps)
            {
                // Cubic case - [a3]x^3 + [a2]x^2 + [a1]x + [a0] = 0
                // Roots are the roots of the cubic
                _complexRoots = SolveCubic(a3, a2, a1, a0);

                foreach (Complex t in _complexRoots)
                {
                    if (!Complex.IsNaN(t) && Complex.IsReal(t))
                    { _realRoots.Add(t.Re); }
                }
            }
            else if (Math.Abs(a0) < _eps)
            {
                // Another cubic case - x([a4]x^3 + [a3]x^2 + [a2]x + [a1]) = 0
                // Roots are x = 0 and the roots of the cubic
                _complexRoots = SolveCubic(a4, a3, a2, a1);

                foreach (Complex t in _complexRoots)
                {
                    if (!Complex.IsNaN(t) && Complex.IsReal(t))
                    { _realRoots.Add(t.Re); }
                }
            }

            // Depress the quartic to x^4 + Ax^3 + Bx^2 + Cx + D = 0
            // Divide all coefficients by the leading coefficient (a4)
            double A = a3 / a4, B = a2 / a4, C = a1 / a4, D = a0 / a4;
            double aOver4 = A * 0.25;

            if (Math.Abs(D) < _eps)
            {
                // Another cubic case - x(x^3 + Ax^2 + Bx + C) = 0
                // Roots are x = 0 and the roots of the cubic
                _complexRoots = SolveCubic(1.0, A, B, C);

                foreach (Complex t in _complexRoots)
                {
                    if (!Complex.IsNaN(t) && Complex.IsReal(t))
                    { _realRoots.Add(t.Re); }
                }
            }

            // Eliminate cubic term to get y^4 + Ey^2 + Fy + G = 0
            // Substitute x = y - A/4 
            double E = B - (3 * A * A) / 8, F = C + (A * A * A) / 8 - (A * B) / 2, G = D - (3 * A * A * A * A) / 256 + (A * A * B) / 16 - (A * C) / 4;

            // Check cases
            if (Math.Abs(G) < _eps)
            {
                // Reduced Cubic case - y(y^3 + Ey + f) = 0
                // Roots are x = -A/4 and the roots of the cubic (minus A/4 each)
                _complexRoots = SolveCubic(1.0, 0, E, F);

                _realRoots.Add(-A * 0.25);
                foreach (Complex t in _complexRoots)
                {
                    if (!Complex.IsNaN(t) && Complex.IsReal(t))
                    { _realRoots.Add(t.Re - A * 0.25); }
                }
            }
            else if (Math.Abs(F) < _eps)
            {
                // BiQuadratic case - y^4 + Ey^2 + G = 0 - complete the square
                // Roots are the four roots of the bi-quadratic (minus A/4 each)
                Complex sqrt = Complex.Sqrt(-G + (E * E) * 0.25);
                Complex sqrtOne = Complex.Sqrt(sqrt - E * 0.5);
                Complex sqrtTwo = Complex.Sqrt(-sqrt - E * 0.5);

                Complex r1 = sqrtOne - aOver4;
                Complex r2 = sqrtTwo - aOver4;
                Complex r3 = -sqrtOne - aOver4;
                Complex r4 = -sqrtTwo - aOver4;

                if (!Complex.IsNaN(r1) && Complex.IsReal(r1))
                    _realRoots.Add(r1.Re);
                if (!Complex.IsNaN(r2) && Complex.IsReal(r2))
                    _realRoots.Add(r2.Re);
                if (!Complex.IsNaN(r3) && Complex.IsReal(r3))
                    _realRoots.Add(r3.Re);
                if (!Complex.IsNaN(r4) && Complex.IsReal(r4))
                    _realRoots.Add(r4.Re);
            }

            // Auxiliary Cubic z^3 + hz^2 + iz - j = 0
            double h = E / 2, i = (E * E - 4 * G) / 16, j = -(F * F) / 64;

            _complexRoots = SolveCubic(1.0, h, i, j);

            if (_complexRoots.Count < 2)
                return _realRoots;

            Complex p = Complex.Sqrt(_complexRoots[0]);
            Complex q = Complex.Sqrt(_complexRoots[1]);

            Complex r = -F / (8 * p * q);

            Complex x1 = p + q + r - aOver4;
            Complex x2 = p - q - r - aOver4;
            Complex x3 = -p + q - r - aOver4;
            Complex x4 = -p - q + r - aOver4;

            if (!Complex.IsNaN(x1) && Complex.IsReal(x1))
                _realRoots.Add(x1.Re);
            if (!Complex.IsNaN(x2) && Complex.IsReal(x2))
                _realRoots.Add(x2.Re);
            if (!Complex.IsNaN(x3) && Complex.IsReal(x3))
                _realRoots.Add(x3.Re);
            if (!Complex.IsNaN(x4) && Complex.IsReal(x4))
                _realRoots.Add(x4.Re);

            return _realRoots;
        }

        // MathForum.org solution for cubic equation
        List<Complex> SolveCubic(double a3, double a2, double a1, double a0)
        {

            if (Math.Abs(a3) < _eps)
            {
                // Quadratic case
                _complexRoots = SolveQuadratic(a2, a1, a0);
                return _complexRoots;
            }

            // Divide all coefficients by leading coefficient (a3 coeff)
            double A = a2 / a3, B = a1 / a3, C = a0 / a3;

            // Substitute (t-A/3) for x to reduce the cubic to t^3 + Pt + Q = 0
            double P = ((3 * B) - (A * A)) / 3;
            double Q = ((2 * A * A * A) - (9 * A * B) + (27 * C)) / 27;

            // Cube roots of unity
            Complex unity_1 = (-1 + Complex.Sqrt(-3)) * 0.5;
            Complex unity_2 = (-1 - Complex.Sqrt(-3)) * 0.5;

            if (Math.Abs(P) < _eps && Math.Abs(Q) < _eps)
            {
                // t is 0 also
                Complex t1 = new Complex(-A / 3, 0);
                _complexRoots.Add(t1);
                return _complexRoots;
            }
            else if (Math.Abs(P) < _eps || Math.Abs(Q) < _eps)
            {
                // Immediately solvable
                if (Math.Abs(P) < _eps)
                {
                    // Equation becomes t^3 + Q = 0, so t = cbrt(-Q)
                    // Subtract A/3 from all answers
                    double aOver3 = A / 3;
                    Complex cPowQ = Complex.Pow(-Q, 1.0 / 3.0);
                    Complex t1 = cPowQ - aOver3;
                    Complex t2 = (cPowQ * unity_1) - aOver3;
                    Complex t3 = (cPowQ * unity_2) - aOver3;

                    _complexRoots.Add(t1);
                    _complexRoots.Add(t2);
                    _complexRoots.Add(t3);
                    return _complexRoots;
                }
                else if (Math.Abs(Q) < _eps)
                {
                    // Equation becomes t^3 + Pt = 0 --> factor out a t to get t(t^2 + P) = 0, so t = 0 and t = +/- sqrt(-P)
                    // Subtract A/3 from all answers
                    double aOver3 = A / 3;
                    Complex cPowP = Complex.Pow(-P, 1.0 / 2.0);
                    Complex t1 = new Complex(-aOver3, 0);
                    Complex t2 = cPowP - aOver3;
                    Complex t3 = -cPowP - aOver3;

                    _complexRoots.Add(t1);
                    _complexRoots.Add(t2);
                    _complexRoots.Add(t3);
                    return _complexRoots;
                }
            }

            if (Math.Abs((A * A) - (3 * B)) < _eps)
            {
                // Perfect cube, factors down to (x + A/3)^3 = A^3/27 - C
                // Subtract A/3 from all answers
                double aOver3 = A / 3;
                Complex cPowA = Complex.Pow(((A * A * A) / 27) - C, 1.0 / 3.0);
                Complex t1 = cPowA - aOver3;
                Complex t2 = (cPowA * unity_1) - aOver3;
                Complex t3 = (cPowA * unity_2) - aOver3;

                _complexRoots.Add(t1);
                _complexRoots.Add(t2);
                _complexRoots.Add(t3);
                return _complexRoots;
            }

            double g = (A * A) - (3 * B);
            double h = (A * B) - (9 * C);
            double i = (B * B) - (3 * A * C);

            Complex sqrt = Complex.Sqrt((h * h) - (4 * g * i));
            Complex z1 = (-h + sqrt) / (2 * g);
            Complex z2 = (-h - sqrt) / (2 * g);
            Complex z = z1.Re > 0 ? (z2.Re > 0 ? (z1.Re < z2.Re ? z1 : z2) : z1) : z2;

            Complex d = (3 * z) + A;
            Complex e = (3 * z * z) + (2 * A * z) + B;
            Complex f = (z * z * z) + (A * z * z) + (B * z) + C;
            Complex cPow = Complex.Pow((e * e * e) - (27 * f * f), 1.0 / 3.0);

            // The three roots are...
            Complex r1 = z + (3 * f) / (cPow - e);
            Complex r2 = z + (3 * f) / ((cPow * unity_1) - e);
            Complex r3 = z + (3 * f) / ((cPow * unity_2) - e);

            _complexRoots.Add(r1);
            _complexRoots.Add(r2);
            _complexRoots.Add(r3);
            return _complexRoots;
        }

        List<Complex> SolveQuadratic(double a2, double a1, double a0)
        {
            if (a2 < _eps)
            {
                // Linear case, bx + c = 0, x = -c/b
                Complex t1 = new Complex(-a0 / a1, 0);

                _complexRoots.Add(t1);
                return _complexRoots;
            }

            // Divide by leading coefficient to eliminate issues
            double b = a1 / a2, c = a0 / a2;

            if (b < _eps)
            {
                // x^2 + c = 0, x = sqrt(-c)
                Complex t1 = Complex.Sqrt(-c);

                _complexRoots.Add(t1);
                return _complexRoots;
            }

            double D = (b * b) - (4 * c);
            Complex sqrtD = Complex.Sqrt(D);

            Complex r1 = (-b + sqrtD) * 0.5;
            Complex r2 = (-b - sqrtD) * 0.5;

            _complexRoots.Add(r1);
            _complexRoots.Add(r2);
            return _complexRoots;
        }
    }
}
