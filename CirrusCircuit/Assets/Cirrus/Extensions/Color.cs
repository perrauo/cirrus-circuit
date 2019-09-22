using UnityEngine.UI;

namespace Cirrus.Extensions
{
    public static class ColorExtension
    {
        public static UnityEngine.Color Copy(UnityEngine.Color c)
        {
            return new UnityEngine.Color(c.r, c.g, c.b, c.a);
        }

        public static void SetR(this Image im, float r)
        {
            var c = im.color;
            c.r = r;
            im.color = c;
        }

        public static void SetG(this Image im, float g)
        {
            var c = im.color;
            c.g = g;
            im.color = c;
        }

        public static void SetB(this Image im, float b)
        {
            var c = im.color;
            c.b = b;
            im.color = c;
        }

        public static void SetA(this Image im, float a)
        {
            var c = im.color;
            c.a = a;
            im.color = c;
        }

        public static UnityEngine.Color SetG(this UnityEngine.Color im, float g)
        {
            var c = im;
            c.g = g;
            im = c;
            return im;
        }

        public static UnityEngine.Color SetB(this UnityEngine.Color im, float b)
        {
            var c = im;
            c.b = b;
            im = c;
            return im;
        }

        public static UnityEngine.Color SetA(this UnityEngine.Color im, float a)
        {
            var c = im;
            c.a = a;
            im = c;
            return im;
        }


        public static UnityEngine.Color Set(this UnityEngine.Color im, float r, float g, float b, float a)
        {
            im.r = r;
            im.g = g;
            im.b = b;
            im.a = a;
            return im;
        }

        public static UnityEngine.Color Set(this UnityEngine.Color im, float r, float g, float b)
        {
            im.r = r;
            im.g = g;
            im.b = b;
            //im.a = a;
            return im;
        }
    }

}