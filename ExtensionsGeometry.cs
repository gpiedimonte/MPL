using System;

namespace System
{
    public static class GeoGeometry
    {

       
        /// <summary>
        /// Earth radius in Km
        /// </summary>
        const int R = 6371;

        /// <summary>
        /// Converte una coordinata in un radiante
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static decimal ToRad(this decimal degree)
        {
            // Converts numeric degrees to radians 
            return degree * (decimal)Math.PI / 180;
        }

        /// <summary>
        /// Converte un radiante in una coordinata
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static decimal ToDeg(this decimal radian)
        {
            return radian * 180 / (decimal)Math.PI;
        }


        public static double DistanceBetween(double thisLat, double thisLon, double pointLat, double pointLon)
        {  
            var lat1 = thisLat.ToRad();
            var lat2 = pointLat.ToRad();
            var dLat = (pointLat - thisLat).ToRad();
            var dLon = Math.Abs(pointLon - thisLon).ToRad();

            var dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
            var q = Double.IsNaN(dLat / dPhi) ? dLat / dPhi : Math.Cos(lat1);  // E-W line gives dPhi=0

            // if dLon over 180° take shorter rhumb across 180° meridian:
            if (dLon > Math.PI)
                dLon = 2 * Math.PI - dLon;
            var dist = Math.Sqrt(dLat * dLat + q * q * dLon * dLon) * R;
            if(double.IsNaN(dist))
                dist = 0;
            return dist;//.toPrecisionFixed(4);  // 4 sig figs reflects typical 0.3% accuracy of spherical model
        }

        //public static DistanceBetweenTwoCoords()
        //{
        //    const int R = 6371;
        //    var dLat = (lat2-lat1).toRad();
        //    var dLon = (lon2-lon1).toRad();
        //    var lat1 = lat1.toRad();
        //    var lat2 = lat2.toRad();

        //    var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
        //            Math.sin(dLon/2) * Math.sin(dLon/2) * Math.cos(lat1) * Math.cos(lat2); 
        //    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a)); 
        //    var d = R * c;
        //}

        //public class Rad 
        //{

        //    public double Hours { get; set; }
        //    public double Minutes { get; set; }
        //    public double Seconds { get; set; }

        //    public override string  ToString()
        //    {
        //        var strBuilder = new StringBuilder();
        //        strBuilder.Append(Hours).Append("°");
        //        strBuilder.Append(Minutes).Append("'");
        //        strBuilder.Append(Seconds).Append(@"″");
        //        return strBuilder.ToString();
        //    }
        //}
 
    }
}