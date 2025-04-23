using System.Globalization;

namespace TravelingApp.Application.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convierte un objeto a un entero.
        /// </summary>
        /// <param name="value">El objeto a convertir.</param>
        /// <returns>El valor entero resultante.</returns>
        public static int ToInt(this object value)
        {
            return int.Parse(value.ToString()!);
        }

        /// <summary>
        /// Convierte un valor de enumeración a su valor entero correspondiente.
        /// </summary>
        /// <param name="enunValue">El valor de la enumeración a convertir.</param>
        /// <returns>El valor entero correspondiente al valor de la enumeración.</returns>
        public static int ToInt(this Enum enunValue)
        {
            return Convert.ToInt32(enunValue);
        }

        /// <summary>
        /// Convierte un objeto a un entero.
        /// </summary>
        /// <param name="value">El objeto a convertir.</param>
        /// <returns>El valor entero resultante.</returns>
        public static long ToLong(this object value)
        {
            return long.Parse(value.ToString()!);
        }

        /// <summary>
        /// Convierte un objeto a un valor decimal.
        /// </summary>
        /// <param name="value">El objeto a convertir.</param>
        /// <returns>El valor decimal resultante.</returns>
        public static decimal ToDecimal(this object value)
        {
            return decimal.Parse(value.ToString()!);
        }

        /// <summary>
        /// Convierte un objeto a un valor double.
        /// </summary>
        /// <param name="value">El objeto a convertir.</param>
        /// <returns>El valor double resultante.</returns>
        public static double ToDouble(this object value)
        {
            return double.Parse(value.ToString()!);
        }

        /// <summary>
        /// Intenta convertir un objeto a un valor decimal.
        /// </summary>
        /// <param name="value">El objeto a intentar convertir.</param>
        /// <returns>True si la conversión fue exitosa; de lo contrario, false.</returns>
        public static bool TryParseDecimal(this object value)
        {
            decimal decimalValue = 0;
            return decimal.TryParse(value.ToString(), out decimalValue);
        }

        /// <summary>
        /// Convierte un objeto a un GUID.
        /// </summary>
        /// <param name="value">El objeto a convertir.</param>
        /// <returns>El GUID resultante.</returns>
        public static Guid ToGuid(this object value)
        {
            return Guid.Parse(value.ToString()!);
        }

        /// <summary>
        /// Intenta convertir un objeto a un valor entero.
        /// </summary>
        /// <param name="value">El objeto a intentar convertir.</param>
        /// <returns>True si la conversión fue exitosa; de lo contrario, false.</returns>
        public static bool TryParseInt(this object value)
        {
            int integerValue = 0;
            return int.TryParse(value.ToString(), out integerValue);
        }

        /// <summary>
        /// Valida que el argumento de un constructor no sea nulo.
        /// </summary>
        /// <typeparam name="T">El tipo del argumento a validar.</typeparam>
        /// <param name="constructorArgument">El argumento a validar.</param>
        /// <returns>El argumento si no es nulo.</returns>
        /// <exception cref="ArgumentNullException">Lanza una excepción si el argumento es nulo.</exception>
        public static T ValidateArgument<T>(this T constructorArgument)
        {
            if (constructorArgument == null)
            {
                throw new ArgumentNullException(typeof(T).Name);
            }

            return constructorArgument;
        }

        /// <summary>
        /// Convierte una cadena de fecha en formato 'dd/MM/yyyy' a 'yyyyMMdd'.
        /// </summary>
        public static string ToCompactDate(this string value)
        {
            if (DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date.ToString("yyyyMMdd");
            }

            return string.Empty;
        }

        /// <summary>
        /// Convierte una hora en formato 'HH:mm:ss' o similar a 'HHmmss'.
        /// </summary>
        public static string ToCompactTime(this string value)
        {
            if (TimeSpan.TryParse(value.Trim(), out var time))
            {
                return time.ToString("hhmmss");
            }

            return string.Empty;
        }
    }
}
