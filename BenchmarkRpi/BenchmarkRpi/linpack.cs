using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BenchmarkRpi
{

    public class LinpackResult
    {
        public double MFlops { get; set; }
        public double Time { get; set; }
        public double Precision { get; set; }
        public double NormRes { get; set; }
    }

    public class Linpack
    {
        private double _secondOrig = -1;

        private static double Abs(double d)
        {
            return (d >= 0) ? d : -d;
        }

        private double Second()
        {
            if (_secondOrig == -1)
            {
                _secondOrig = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }
            return (DateTime.Now.TimeOfDay.TotalMilliseconds - _secondOrig) / 1000;
        }

        public LinpackResult run_benchmark()
        {
            double mflopsResult;
            double residnResult;
            double timeResult;
            double epsResult;

            var a = new double[200][];
            for (var q = 0; q < a.Length; q++)
                a[q] = new double[201];
            var b = new double[200];
            var x = new double[200];
            double ops, total, norma, normx;
            double resid, time;
            int n, i, lda;
            var ipvt = new int[500];

            lda = 201; //201
            n = 200;  //100

            // ops = (2.0e0 * (((double)n) * n * n)) / 3.0 + 2.0 * (n * n);

            ops = (2.0e0 * (n * n * n)) / 3.0 + 2.0 * (n * n);

            Matgen(a, lda, n, b);
            time = Second();
            Dgefa(a, lda, n, ipvt);
            Dgesl(a, lda, n, ipvt, b, 0);
            total = Second() - time;

            for (i = 0; i < n; i++)
            {
                x[i] = b[i];
            }
            norma = Matgen(a, lda, n, b);
            for (i = 0; i < n; i++)
            {
                b[i] = -b[i];
            }
            dmxpy(n, b, n, lda, x, a);
            resid = 0.0;
            normx = 0.0;
            for (i = 0; i < n; i++)
            {
                resid = (resid > Abs(b[i])) ? resid : Abs(b[i]);
                normx = (normx > Abs(x[i])) ? normx : Abs(x[i]);
            }

            epsResult = epslon(1.0);
            residnResult = resid / (n * norma * normx * epsResult);
            residnResult += 0.005; // for rounding
            residnResult = (int)(residnResult * 100);
            residnResult /= 100;

            timeResult = total;
            timeResult += 0.005; // for rounding
            timeResult = (int)(timeResult * 100);
            timeResult /= 100;

            mflopsResult = ops / (1.0e6 * total);
            mflopsResult += 0.0005; // for rounding
            mflopsResult = (int)(mflopsResult * 1000);
            mflopsResult /= 1000;

            //Debug.WriteLine("Mflops/s: " + mflopsResult +
            //                "  Time: " + timeResult + " secs" +
            //                "  Norm Res: " + residnResult +
            //                "  Precision: " + epsResult);

            return new LinpackResult
            {
                MFlops = mflopsResult,
                Time = timeResult,
                NormRes = residnResult,
                Precision = epsResult
            };
        }


        private static double Matgen(IList<double[]> a, int lda, int n, IList<double> b)
        {
            double norma;
            int init, i, j;

            init = 1325;
            norma = 0.0;
            /*  Next two for() statements switched.  Solver wants
            matrix in column order. --dmd 3/3/97
            */
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                {
                    init = 3125 * init % 65536;
                    a[j][i] = (init - 32768.0) / 16384.0;
                    norma = (a[j][i] > norma) ? a[j][i] : norma;
                }
            }
            for (i = 0; i < n; i++)
            {
                b[i] = 0.0;
            }
            for (j = 0; j < n; j++)
            {
                for (i = 0; i < n; i++)
                {
                    b[i] += a[j][i];
                }
            }

            return norma;
        }


        /*
        dgefa factors a double precision matrix by gaussian elimination.
    
        dgefa is usually called by dgeco, but it can be called
        directly with a saving in time if  rcond  is not needed.
        (time for dgeco) = (1 + 9/n)*(time for dgefa) .
    
        on entry
    
        a       double precision[n][lda]
        the matrix to be factored.
    
        lda     integer
        the leading dimension of the array  a .
    
        n       integer
        the order of the matrix  a .
    
        on return
    
        a       an upper triangular matrix and the multipliers
        which were used to obtain it.
        the factorization can be written  a = l*u  where
        l  is a product of permutation and unit lower
        triangular matrices and  u  is upper triangular.
    
        ipvt    integer[n]
        an integer vector of pivot indices.
    
        info    integer
        = 0  normal value.
        = k  if  u[k][k] .eq. 0.0 .  this is not an error
        condition for this subroutine, but it does
        indicate that dgesl or dgedi will divide by zero
        if called.  use  rcond  in dgeco for a reliable
        indication of singularity.
    
        linpack. this version dated 08/14/78.
        cleve moler, university of new mexico, argonne national lab.
    
        functions
    
        blas daxpy,dscal,idamax
        */

        private int Dgefa(double[][] a, int lda, int n, IList<int> ipvt)
        {
            int nm1;
            int info;

            // gaussian elimination with partial pivoting

            info = 0;
            nm1 = n - 1;
            if (nm1 >= 0)
            {
                int k;
                for (k = 0; k < nm1; k++)
                {
                    var col_k = a[k];
                    var kp1 = k + 1;

                    // find l = pivot index

                    var l = idamax(n - k, col_k, k, 1) + k;
                    ipvt[k] = l;

                    // zero pivot implies this column already triangularized

                    if (col_k[l] != 0)
                    {
                        // interchange if necessary

                        double t;
                        if (l != k)
                        {
                            t = col_k[l];
                            col_k[l] = col_k[k];
                            col_k[k] = t;
                        }

                        // compute multipliers

                        t = -1.0 / col_k[k];
                        Dscal(n - (kp1), t, col_k, kp1, 1);

                        // row elimination with column indexing

                        int j;
                        for (j = kp1; j < n; j++)
                        {
                            double[] col_j = a[j];
                            t = col_j[l];
                            if (l != k)
                            {
                                col_j[l] = col_j[k];
                                col_j[k] = t;
                            }
                            Daxpy(n - (kp1), t, col_k, kp1, 1,
                                col_j, kp1, 1);
                        }
                    }
                    else
                    {
                        info = k;
                    }
                }
            }
            ipvt[n - 1] = n - 1;
            if (a[(n - 1)][(n - 1)] == 0) info = n - 1;

            return info;
        }


        /*
        dgesl solves the double precision system
        a * x = b  or  trans(a) * x = b
        using the factors computed by dgeco or dgefa.
  
        on entry
  
        a       double precision[n][lda]
        the output from dgeco or dgefa.
  
        lda     integer
        the leading dimension of the array  a .
    
        n       integer
        the order of the matrix  a .
  
        ipvt    integer[n]
        the pivot vector from dgeco or dgefa.

        b       double precision[n]
        the right hand side vector.
    
        job     integer
        = 0         to solve  a*x = b ,
        = nonzero   to solve  trans(a)*x = b  where
        trans(a)  is the transpose.
    
        on return
    
        b       the solution vector  x .
    
        error condition
    
        a division by zero will occur if the input factor contains a
        zero on the diagonal.  technically this indicates singularity
        but it is often caused by improper arguments or improper
        setting of lda .  it will not occur if the subroutines are
        called correctly and if dgeco has set rcond .gt. 0.0
        or dgefa has set info .eq. 0 .
    
        to compute  inverse(a) * c  where  c  is a matrix
        with  p  columns
        dgeco(a,lda,n,ipvt,rcond,z)
        if (!rcond is too small){
        for (j=0,j<p,j++)
        dgesl(a,lda,n,ipvt,c[j][0],0);
        }
    
        linpack. this version dated 08/14/78 .
        cleve moler, university of new mexico, argonne national lab.
    
        functions
    
        blas daxpy,ddot
        */

        private void Dgesl(double[][] a, int lda, int n, int[] ipvt, double[] b, int job)
        {
            double t;
            int k, kb, l, nm1, kp1;

            nm1 = n - 1;
            if (job == 0)
            {
                // job = 0 , solve  a * x = b.  first solve  l*y = b

                if (nm1 >= 1)
                {
                    for (k = 0; k < nm1; k++)
                    {
                        l = ipvt[k];
                        t = b[l];
                        if (l != k)
                        {
                            b[l] = b[k];
                            b[k] = t;
                        }
                        kp1 = k + 1;
                        Daxpy(n - (kp1), t, a[k], kp1, 1, b, kp1, 1);
                    }
                }

                // now solve  u*x = y

                for (kb = 0; kb < n; kb++)
                {
                    k = n - (kb + 1);
                    b[k] /= a[k][k];
                    t = -b[k];
                    Daxpy(k, t, a[k], 0, 1, b, 0, 1);
                }
            }
            else
            {
                // job = nonzero, solve  trans(a) * x = b.  first solve  trans(u)*y = b

                for (k = 0; k < n; k++)
                {
                    t = Ddot(k, a[k], 0, 1, b, 0, 1);
                    b[k] = (b[k] - t) / a[k][k];
                }

                // now solve trans(l)*x = y 

                if (nm1 >= 1)
                {
                    for (kb = 1; kb < nm1; kb++)
                    {
                        k = n - (kb + 1);
                        kp1 = k + 1;
                        b[k] += Ddot(n - (kp1), a[k], kp1, 1, b, kp1, 1);
                        l = ipvt[k];
                        if (l != k)
                        {
                            t = b[l];
                            b[l] = b[k];
                            b[k] = t;
                        }
                    }
                }
            }
        }


        /*
        constant times a vector plus a vector.
        jack dongarra, linpack, 3/11/78.
        */

        private static void Daxpy(int n, double da, double[] dx, int dxOff, int incx,
            double[] dy, int dyOff, int incy)
        {
            if ((n <= 0) || (da == 0)) return;

            int i;
            if (incx != 1 || incy != 1)
            {
                // code for unequal increments or equal increments not equal to 1

                var ix = 0;
                var iy = 0;
                if (incx < 0) ix = (-n + 1) * incx;
                if (incy < 0) iy = (-n + 1) * incy;
                for (i = 0; i < n; i++)
                {
                    dy[iy + dyOff] += da * dx[ix + dxOff];
                    ix += incx;
                    iy += incy;
                }
            }
            // code for both increments equal to 1

            for (i = 0; i < n; i++)
                dy[i + dyOff] += da * dx[i + dxOff];
        }


        /*
        forms the dot product of two vectors.
        jack dongarra, linpack, 3/11/78.
        */

        private static double Ddot(int n, double[] dx, int dxOff, int incx, double[] dy,
            int dyOff, int incy)
        {
            double dtemp;

            dtemp = 0;

            if (n <= 0) return (dtemp);

            int i;
            if (incx != 1 || incy != 1)
            {
                // code for unequal increments or equal increments not equal to 1

                var ix = 0;
                var iy = 0;
                if (incx < 0) ix = (-n + 1) * incx;
                if (incy < 0) iy = (-n + 1) * incy;
                for (i = 0; i < n; i++)
                {
                    dtemp += dx[ix + dxOff] * dy[iy + dyOff];
                    ix += incx;
                    iy += incy;
                }
            }
            else
            {
                // code for both increments equal to 1

                for (i = 0; i < n; i++)
                    dtemp += dx[i + dxOff] * dy[i + dyOff];
            }
            return (dtemp);
        }


        /*
        scales a vector by a constant.
        jack dongarra, linpack, 3/11/78.
        */

        private static void Dscal(int n, double da, double[] dx, int dxOff, int incx)
        {
            if (n > 0)
            {
                int i;
                if (incx != 1)
                {
                    // code for increment not equal to 1

                    var nincx = n * incx;
                    for (i = 0; i < nincx; i += incx)
                        dx[i + dxOff] *= da;
                }
                else
                {
                    // code for increment equal to 1

                    for (i = 0; i < n; i++)
                        dx[i + dxOff] *= da;
                }
            }
        }


        /*
        finds the index of element having max. absolute value.
        jack dongarra, linpack, 3/11/78.
        */

        private int idamax(int n, double[] dx, int dx_off, int incx)
        {
            double dmax, dtemp;
            int i, ix, itemp = 0;

            if (n < 1)
            {
                itemp = -1;
            }
            else if (n == 1)
            {
                itemp = 0;
            }
            else if (incx != 1)
            {
                // code for increment not equal to 1

                dmax = Abs(dx[0 + dx_off]);
                ix = 1 + incx;
                for (i = 1; i < n; i++)
                {
                    dtemp = Abs(dx[ix + dx_off]);
                    if (dtemp > dmax)
                    {
                        itemp = i;
                        dmax = dtemp;
                    }
                    ix += incx;
                }
            }
            else
            {
                // code for increment equal to 1

                itemp = 0;
                dmax = Abs(dx[0 + dx_off]);
                for (i = 1; i < n; i++)
                {
                    dtemp = Abs(dx[i + dx_off]);
                    if (dtemp > dmax)
                    {
                        itemp = i;
                        dmax = dtemp;
                    }
                }
            }
            return (itemp);
        }


        /*
        estimate unit roundoff in quantities of size x.
    
        this program should function properly on all systems
        satisfying the following two assumptions,
        1.  the base used in representing dfloating point
        numbers is not a power of three.
        2.  the quantity  a  in statement 10 is represented to
        the accuracy used in dfloating point variables
        that are stored in memory.
        the statement number 10 and the go to 10 are intended to
        force optimizing compilers to generate code satisfying
        assumption 2.
        under these assumptions, it should be true that,
        a  is not exactly equal to four-thirds,
        b  has a zero for its last bit or digit,
        c  is not exactly equal to one,
        eps  measures the separation of 1.0 from
        the next larger dfloating point number.
        the developers of eispack would appreciate being informed
        about any systems where these assumptions do not hold.
    
        *****************************************************************
        this routine is one of the auxiliary routines used by eispack iii
        to avoid machine dependencies.
        *****************************************************************
  
        this version dated 4/6/83.
        */

        private double epslon(double x)
        {
            double a, b, c, eps;

            a = 4.0e0 / 3.0e0;
            eps = 0;
            while (eps == 0)
            {
                b = a - 1.0;
                c = b + b + b;
                eps = Abs(c - 1.0);
            }
            return (eps * Abs(x));
        }


        /*
        purpose:
        multiply matrix m times vector x and add the result to vector y.
    
        parameters:
    
        n1 integer, number of elements in vector y, and number of rows in
        matrix m
    
        y double [n1], vector of length n1 to which is added
        the product m*x
    
        n2 integer, number of elements in vector x, and number of columns
        in matrix m
    
        ldm integer, leading dimension of array m
    
        x double [n2], vector of length n2
    
        m double [ldm][n2], matrix of n1 rows and n2 columns
        */

        private void dmxpy(int n1, double[] y, int n2, int ldm, double[] x, double[][] m)
        {
            // cleanup odd vector
            for (var j = 0; j < n2; j++)
            {
                int i;
                for (i = 0; i < n1; i++)
                {
                    y[i] += x[j] * m[j][i];
                }
            }
        }
    }
}
